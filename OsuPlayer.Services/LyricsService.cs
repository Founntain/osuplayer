using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using OsuPlayer.Interfaces.Service;

namespace OsuPlayer.Services;

public sealed class LyricsService : ILyricsService
{
    private static readonly Uri SearchUri = new("https://music.163.com/api/cloudsearch/pc");
    private static readonly Uri RefererUri = new("https://music.163.com/");
    private static readonly TimeSpan NegativeCacheLifetime = TimeSpan.FromDays(7);
    private static readonly Regex VersionSuffixRegex = new(
        @"(?:\s*[-–—]\s*)?[\(\[\{（【]\s*(?:tv\s*(?:size|ver(?:sion)?|edit)|short\s*(?:ver(?:sion)?)?|anime\s*(?:ver(?:sion)?)?|instrumental|off\s*vocal|karaoke|伴奏)\s*[\)\]\}）】]\s*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex FeatureArtistRegex = new(
        @"\s+(?:feat\.?|ft\.?|featuring|with|×|x)\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly HttpClient _httpClient;
    private readonly string _cacheDirectory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public LyricsService(HttpClient? httpClient = null, string? cacheDirectory = null)
    {
        _httpClient = httpClient ?? CreateHttpClient();
        _cacheDirectory = cacheDirectory ?? Path.Combine("data", "lyrics");
    }

    public async Task<LyricsLookupResult> GetLyricsAsync(string title, string artist,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return CreateResult(LyricsLookupStatus.NotFound, "No song title is available.");

        var cleanedTitle = CleanTitle(title);
        var cleanedArtist = CleanArtist(artist);
        var cachePath = GetCachePath(cleanedTitle, cleanedArtist);

        try
        {
            var cached = await ReadCacheAsync(cachePath, cancellationToken);
            if (cached != null && IsCacheUsable(cached))
                return cached.ToResult(fromCache: true);

            var match = await SearchBestMatchAsync(cleanedTitle, cleanedArtist, cancellationToken);
            if (match == null)
            {
                var notFound = CreateResult(LyricsLookupStatus.NotFound,
                    "No matching song was found on NetEase Cloud Music.");
                await WriteCacheAsync(cachePath, LyricsCacheEntry.FromResult(cleanedTitle, cleanedArtist, notFound),
                    cancellationToken);
                return notFound;
            }

            var result = await DownloadLyricsAsync(match, cancellationToken);
            await WriteCacheAsync(cachePath, LyricsCacheEntry.FromResult(cleanedTitle, cleanedArtist, result),
                cancellationToken);
            return result;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            return CreateResult(LyricsLookupStatus.Unavailable,
                $"Lyrics are temporarily unavailable: {ex.Message}");
        }
    }

    private async Task<SongMatch?> SearchBestMatchAsync(string title, string artist,
        CancellationToken cancellationToken)
    {
        var query = string.IsNullOrWhiteSpace(artist) ? title : $"{title} {artist}";
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["s"] = query,
            ["type"] = "1",
            ["offset"] = "0",
            ["limit"] = "20"
        });
        using var response = await _httpClient.PostAsync(SearchUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = document.RootElement;
        if (!root.TryGetProperty("code", out var code) || code.GetInt32() != 200 ||
            !root.TryGetProperty("result", out var result) ||
            !result.TryGetProperty("songs", out var songs))
            return null;

        SongMatch? best = null;
        foreach (var song in songs.EnumerateArray())
        {
            if (!song.TryGetProperty("id", out var idElement) ||
                !song.TryGetProperty("name", out var nameElement))
                continue;

            var name = nameElement.GetString() ?? string.Empty;
            var artists = ReadArtists(song);
            var score = CalculateMatchScore(title, artist, name, artists);
            if (score.Title < 0.52 || score.Total < 0.62)
                continue;

            var candidate = new SongMatch(idElement.GetInt64(), name, artists, score.Total);
            if (best == null || candidate.Score > best.Score)
                best = candidate;
        }

        return best;
    }

    private async Task<LyricsLookupResult> DownloadLyricsAsync(SongMatch match,
        CancellationToken cancellationToken)
    {
        var uri = new Uri($"https://music.163.com/api/song/lyric?lv=-1&kv=-1&tv=-1&id={match.Id}");
        using var response = await _httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = document.RootElement;

        if (root.TryGetProperty("nolyric", out var noLyric) && noLyric.ValueKind == JsonValueKind.True)
        {
            return new LyricsLookupResult(LyricsLookupStatus.Instrumental, match.Id, match.Title,
                match.Artist, string.Empty, string.Empty, false, "This track is marked as instrumental.");
        }

        var lyrics = ReadNestedString(root, "lrc", "lyric");
        var translation = ReadNestedString(root, "tlyric", "lyric");
        if (string.IsNullOrWhiteSpace(lyrics))
        {
            return new LyricsLookupResult(LyricsLookupStatus.NotFound, match.Id, match.Title,
                match.Artist, string.Empty, string.Empty, false, "No lyrics are available for this track.");
        }

        return new LyricsLookupResult(LyricsLookupStatus.Found, match.Id, match.Title,
            match.Artist, lyrics, translation, false, "Lyrics loaded from NetEase Cloud Music.");
    }

    private async Task<LyricsCacheEntry?> ReadCacheAsync(string cachePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(cachePath)) return null;

        try
        {
            await using var stream = File.OpenRead(cachePath);
            return await JsonSerializer.DeserializeAsync<LyricsCacheEntry>(stream, _jsonOptions, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    private async Task WriteCacheAsync(string cachePath, LyricsCacheEntry entry,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_cacheDirectory);
        var temporaryPath = $"{cachePath}.{Guid.NewGuid():N}.tmp";

        try
        {
            await using (var stream = File.Create(temporaryPath))
                await JsonSerializer.SerializeAsync(stream, entry, _jsonOptions, cancellationToken);

            File.Move(temporaryPath, cachePath, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }

    private string GetCachePath(string title, string artist)
    {
        var key = NormalizeForComparison($"{title}\n{artist}");
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key))).ToLowerInvariant();
        return Path.Combine(_cacheDirectory, $"{hash}.json");
    }

    private static bool IsCacheUsable(LyricsCacheEntry entry)
    {
        if (entry.Version != 1) return false;
        if (entry.Status is LyricsLookupStatus.Found or LyricsLookupStatus.Instrumental) return true;
        return entry.Status == LyricsLookupStatus.NotFound &&
               DateTimeOffset.UtcNow - entry.FetchedAtUtc < NegativeCacheLifetime;
    }

    private static string CleanTitle(string title)
    {
        var cleaned = title.Trim().Trim('"', '\'', '“', '”', '「', '」');
        string previous;
        do
        {
            previous = cleaned;
            cleaned = VersionSuffixRegex.Replace(cleaned, string.Empty).Trim();
        } while (!string.Equals(previous, cleaned, StringComparison.Ordinal));

        cleaned = cleaned.Trim('"', '\'', '“', '”', '「', '」');
        return WhitespaceRegex.Replace(cleaned, " ");
    }

    private static string CleanArtist(string artist)
    {
        return WhitespaceRegex.Replace(artist.Trim(), " ");
    }

    private static (double Title, double Total) CalculateMatchScore(string queryTitle, string queryArtist,
        string candidateTitle, string candidateArtists)
    {
        var titleScore = Similarity(NormalizeForComparison(queryTitle),
            NormalizeForComparison(CleanTitle(candidateTitle)));
        var artistScore = string.IsNullOrWhiteSpace(queryArtist)
            ? 1d
            : ArtistSimilarity(queryArtist, candidateArtists);
        return (titleScore, (titleScore * 0.82) + (artistScore * 0.18));
    }

    private static double ArtistSimilarity(string queryArtist, string candidateArtists)
    {
        var queryParts = FeatureArtistRegex.Split(queryArtist)
            .Select(NormalizeForComparison)
            .Where(x => x.Length > 0)
            .ToArray();
        var candidate = NormalizeForComparison(candidateArtists);
        if (queryParts.Length == 0) return 1;

        return queryParts.Max(part => candidate.Contains(part, StringComparison.Ordinal)
            ? 1d
            : Similarity(part, candidate));
    }

    private static double Similarity(string left, string right)
    {
        if (left == right) return 1;
        if (left.Length == 0 || right.Length == 0) return 0;
        if (left.Contains(right, StringComparison.Ordinal) || right.Contains(left, StringComparison.Ordinal))
            return (double)Math.Min(left.Length, right.Length) / Math.Max(left.Length, right.Length);

        var previous = Enumerable.Range(0, right.Length + 1).ToArray();
        var current = new int[right.Length + 1];
        for (var i = 1; i <= left.Length; i++)
        {
            current[0] = i;
            for (var j = 1; j <= right.Length; j++)
            {
                var cost = left[i - 1] == right[j - 1] ? 0 : 1;
                current[j] = Math.Min(Math.Min(current[j - 1] + 1, previous[j] + 1), previous[j - 1] + cost);
            }

            (previous, current) = (current, previous);
        }

        return 1d - ((double)previous[right.Length] / Math.Max(left.Length, right.Length));
    }

    private static string NormalizeForComparison(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormKC).ToLowerInvariant();
        var builder = new StringBuilder(normalized.Length);
        foreach (var character in normalized.Where(char.IsLetterOrDigit))
            builder.Append(character);
        return builder.ToString();
    }

    private static string ReadArtists(JsonElement song)
    {
        if (!song.TryGetProperty("ar", out var artists) && !song.TryGetProperty("artists", out artists))
            return string.Empty;

        return string.Join(", ", artists.EnumerateArray()
            .Select(x => x.TryGetProperty("name", out var name) ? name.GetString() : null)
            .Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static string ReadNestedString(JsonElement root, string objectName, string propertyName)
    {
        return root.TryGetProperty(objectName, out var nested) &&
               nested.ValueKind == JsonValueKind.Object &&
               nested.TryGetProperty(propertyName, out var value)
            ? value.GetString() ?? string.Empty
            : string.Empty;
    }

    private static LyricsLookupResult CreateResult(LyricsLookupStatus status, string message)
    {
        return new LyricsLookupResult(status, null, string.Empty, string.Empty,
            string.Empty, string.Empty, false, message);
    }

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(12) };
        client.DefaultRequestHeaders.Referrer = RefererUri;
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 osuplayer-elite/1.0");
        return client;
    }

    private sealed record SongMatch(long Id, string Title, string Artist, double Score);

    private sealed class LyricsCacheEntry
    {
        public int Version { get; init; } = 1;
        public string QueryTitle { get; init; } = string.Empty;
        public string QueryArtist { get; init; } = string.Empty;
        public LyricsLookupStatus Status { get; init; }
        public long? SongId { get; init; }
        public string MatchedTitle { get; init; } = string.Empty;
        public string MatchedArtist { get; init; } = string.Empty;
        public string Lyrics { get; init; } = string.Empty;
        public string TranslatedLyrics { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public DateTimeOffset FetchedAtUtc { get; init; }

        public LyricsLookupResult ToResult(bool fromCache)
        {
            return new LyricsLookupResult(Status, SongId, MatchedTitle, MatchedArtist,
                Lyrics, TranslatedLyrics, fromCache, Message);
        }

        public static LyricsCacheEntry FromResult(string queryTitle, string queryArtist,
            LyricsLookupResult result)
        {
            return new LyricsCacheEntry
            {
                QueryTitle = queryTitle,
                QueryArtist = queryArtist,
                Status = result.Status,
                SongId = result.SongId,
                MatchedTitle = result.MatchedTitle,
                MatchedArtist = result.MatchedArtist,
                Lyrics = result.Lyrics,
                TranslatedLyrics = result.TranslatedLyrics,
                Message = result.Message,
                FetchedAtUtc = DateTimeOffset.UtcNow
            };
        }
    }
}
