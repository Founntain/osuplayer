using System.Text;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace OsuPlayer.IO.DbReader.DataModels;

/// <summary>
/// a full beatmap entry with optionally used data
/// <remarks>only created on a <see cref="DbReader.ReadFullMapEntry" /> call</remarks>
/// </summary>
public class DbMapEntry : DbMapEntryBase, IMapEntry
{
    public string ArtistUnicode { get; set; }
    public string TitleUnicode { get; set; }
    public string AudioFileName { get; set; }
    public int BeatmapSetId { get; set; }
    public string FolderName { get; set; }
    public string FolderPath { get; set; }
    public string FullPath { get; set; }
    public bool UseUnicode { get; set; }

    public async Task<Bitmap?> FindBackground()
    {
        var eventCount = 0;

        if (string.IsNullOrEmpty(FolderPath))
            return null;

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

    public override string GetArtist()
    {
        if (UseUnicode)
            return string.IsNullOrEmpty(ArtistUnicode) ? Artist : ArtistUnicode;
        return Artist;
    }

    public override string GetTitle()
    {
        if (UseUnicode)
            return string.IsNullOrEmpty(TitleUnicode) ? Artist : TitleUnicode;
        return Title;
    }

    public override string GetSongName()
    {
        if (UseUnicode && !string.IsNullOrEmpty(ArtistUnicode) && !string.IsNullOrEmpty(TitleUnicode))
            return $"{ArtistUnicode} - {TitleUnicode}";
        return $"{Artist} - {Title}";
    }

    public override string ToString()
    {
        return GetSongName();
    }

    public static bool operator ==(DbMapEntry? left, DbMapEntry? right)
    {
        return left?.SongName == right?.SongName;
    }

    public static bool operator !=(DbMapEntry? left, DbMapEntry? right)
    {
        return left?.SongName != right?.SongName;
    }

    public override bool Equals(object? obj)
    {
        if (obj is DbMapEntry other) return BeatmapChecksum == other.BeatmapChecksum;

        return false;
    }

    public override int GetHashCode()
    {
        return BitConverter.ToInt32(Encoding.UTF8.GetBytes(BeatmapChecksum));
    }
}