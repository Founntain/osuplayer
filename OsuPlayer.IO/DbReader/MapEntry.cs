using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using OsuPlayer.Extensions;

namespace OsuPlayer.IO.DbReader;

/// <summary>
///     Represets a Beatmap from osu! in our own structure
/// </summary>
public class MapEntry
{
    public int BeatmapId;
    public int BeatmapSetId;
    public string Artist = null!;
    public string? ArtistUnicode;
    public string? AudioFileName;
    public string? BeatmapChecksum;
    public string? Creator;
    public string? DifficultyName;
    public string? FolderName;
    public string? FolderPath;
    public string? Fullpath;
    public string Title = null!;
    public string? TitleUnicode;

    public int TotalTime;

    public bool UseUnicode;

    public int Ver;
    public string ArtistString => GetArtist();
    public string TitleString => GetTitle();

    public string SongName => GetSongName();
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();

    private string GetArtist()
    {
        if (!UseUnicode) return Artist;

        return !string.IsNullOrWhiteSpace(ArtistUnicode) ? ArtistUnicode : Artist;
    }

    private string GetTitle()
    {
        if (!UseUnicode) return Title;

        return !string.IsNullOrWhiteSpace(TitleUnicode) ? TitleUnicode : Title;
    }

    private string GetSongName()
    {
        if (!UseUnicode) return $"{Artist} - {Title}";

        if (!string.IsNullOrWhiteSpace(ArtistUnicode) && !string.IsNullOrWhiteSpace(TitleUnicode))
            return $"{ArtistUnicode} - {TitleUnicode}";

        return $"{Artist} - {Title}";
    }

    public override string ToString()
    {
        return Artist + " - " + Title;
    }

    public async Task<Bitmap?> FindBackground()
    {
        var eventCount = 0;

        // ReSharper disable once AssignNullToNotNullAttribute

        var files = Directory.GetFiles(FolderPath, "*.osu");

        if (files.Length == 0)
            return null;
        if (files[0].Length > 260)
            return null;

        var content = (await File.ReadAllLinesAsync(files[0])).ToArray();

        foreach (var s in content)
        {
            if (s.Equals("[Events]")) break;

            eventCount++;
        }

        var background = string.Empty;

        if (content.Length == 0)
            return null;

        for (var e = 1; e < 6; e++)
            if (content[eventCount + e].ToLower().Contains(".jpg") ||
                content[eventCount + e].ToLower().Contains(".png"))
            {
                background = content[eventCount + e];
                break;
            }

        if (string.IsNullOrEmpty(background))
            return null;

        var fileName = background.Split(',')[2].Replace("\"", string.Empty);

        if (File.Exists(Path.Combine(FolderPath, fileName)))
        {
            await using var stream = File.OpenRead(Path.Combine(FolderPath, fileName));
            return await Task.Run(() => new Bitmap(stream));
        }

        var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
        var asset = assets?.Open(new Uri("avares://OsuPlayer/Resources/defaultBg.jpg"));
        return new Bitmap(asset);
    }

    public static bool operator ==(MapEntry? left, MapEntry? right)
    {
        return left?.SongName == right?.SongName;
    }

    public static bool operator !=(MapEntry? left, MapEntry? right)
    {
        return left?.SongName != right?.SongName;
    }
}