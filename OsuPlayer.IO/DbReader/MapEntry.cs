using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using OsuPlayer.Extensions;

namespace OsuPlayer.IO.DbReader;

public class MapEntry
{
    public string Artist;
    public string ArtistString => GetArtist();
    public string ArtistUnicode;
    public string AudioFileName;
    public string BeatmapChecksum;
    public int BeatmapId;
    public int BeatmapSetId;
    public string Creator;
    public string DifficultyName;
    public string FolderName;
    public string Title;
    public string TitleString => GetTitle();
    public string TitleUnicode;
    public string Fullpath;
    
    public string SongName => GetSongName();

    public int TotalTime;
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();

    public int Ver;

    private string GetArtist()
    {
        if (!Config.GetConfigInstance().UseSongNameUnicode) return Artist;

        return !string.IsNullOrWhiteSpace(ArtistUnicode) ? ArtistUnicode : Artist;
    }

    private string GetTitle()
    {
        if (!Config.GetConfigInstance().UseSongNameUnicode) return Title;

        return !string.IsNullOrWhiteSpace(TitleUnicode) ? TitleUnicode : Title;
    }

    private string GetSongName()
    {
        if (!Config.GetConfigInstance().UseSongNameUnicode) return $"{Artist} - {Title}";

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
        var path = $"{Config.GetConfigInstance().OsuSongsPath}\\{FolderName}";

        var eventCount = 0;

        // ReSharper disable once AssignNullToNotNullAttribute

        var files = Directory.GetFiles(path, "*.osu");

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

        if (File.Exists(Path.Combine(path, fileName)))
        {
            await using var stream = File.OpenRead(Path.Combine(path, fileName));
            return await Task.Run(() => new Bitmap(stream));
        }
        else
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var asset = assets?.Open(new Uri("avares://OsuPlayer/Resources/defaultBg.jpg"));
            return new Bitmap(asset);
        }
    }
}