namespace OsuPlayer.Interfaces.Service;

public interface ILyricsService
{
    Task<LyricsLookupResult> GetLyricsAsync(string title, string artist,
        CancellationToken cancellationToken = default);

    Task<LyricsLookupResult> GetLyricsAsync(LyricsLookupRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record LyricsLookupRequest(
    string Title,
    string Artist,
    string TitleUnicode,
    string ArtistUnicode);

public enum LyricsLookupStatus
{
    Found,
    Instrumental,
    NotFound,
    Unavailable
}

public sealed record LyricsLookupResult(
    LyricsLookupStatus Status,
    long? SongId,
    string MatchedTitle,
    string MatchedArtist,
    string Lyrics,
    string TranslatedLyrics,
    bool FromCache,
    string Message)
{
    public bool HasLyrics => Status == LyricsLookupStatus.Found && !string.IsNullOrWhiteSpace(Lyrics);
}
