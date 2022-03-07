namespace OsuPlayer.IO;

public sealed class SongEntry
{
    public SongEntry(int beatmapSetId, int beatmapId, string checksum, string artist, string artistUnicode,
        string title, string titleUnicode,
        string folderName, string audioFileName, bool isCustomSong = false)
    {
        BeatmapSetId = beatmapSetId;
        BeatmapId = beatmapId;
        Checksum = checksum;
        Artist = isCustomSong ? title[0].ToString() : artist;
        ArtistUnicode = artistUnicode;
        Title = title;
        TitleUnicode = titleUnicode;
        FolderName = folderName;
        AudioFileName = audioFileName;
        Fullpath = !isCustomSong
            ? $"{OsuPlayerConfig.OsuSongsPath}\\{FolderName}\\{AudioFileName}"
            : AudioFileName;
        //Background = !isCustomSong
        //    ? FindBackground()
        //    : string.Empty;
        Background = string.Empty;
        IsCustomSong = isCustomSong;

        if (artist.Length == 0)
            Artist = "Unkown Artist";
        if (title.Length == 0)
            Title = "Unkown Title";
    }

    public int BeatmapSetId { get; set; }
    public int BeatmapId { get; set; }
    public string Checksum { get; set; }

    public string ArtistString => GetArtist();
    public string Artist { get; set; }
    public string ArtistUnicode { get; set; }

    public string TitleString => GetTitle();
    public string Title { get; set; }
    public string TitleUnicode { get; set; }

    public string FolderName { get; set; }
    public string AudioFileName { get; set; }
    public string Fullpath { get; set; }

    public int TotalTime { get; set; }

    public string Background { get; set; }
    public bool IsCustomSong { get; set; }
    public string SongName => GetSongName();
    public string UniCodeSongname => $"{ArtistUnicode} - {TitleUnicode}";
    public string NonUniCodeSongname => $"{Artist} - {Title}";

    public string FindBackground()
    {
        if (IsCustomSong) return string.Empty;

        var path = $"{OsuPlayerConfig.OsuSongsPath}\\{FolderName}";

        var eventCount = 0;

        // ReSharper disable once AssignNullToNotNullAttribute

        var files = Directory.GetFiles(path, "*.osu");

        if (files.Length == 0)
            return string.Empty;
        if (files[0].Length > 260)
            return string.Empty;

        var content = File.ReadAllLines(files[0]).ToArray();

        foreach (var s in content)
        {
            if (s.Equals("[Events]")) break;

            eventCount++;
        }

        var background = string.Empty;

        if (content.Length == 0)
            return string.Empty;

        for (var e = 1; e < 6; e++)
            if (content[eventCount + e].ToLower().Contains(".jpg") ||
                content[eventCount + e].ToLower().Contains(".png"))
            {
                background = content[eventCount + e];
                break;
            }

        if (string.IsNullOrEmpty(background))
            return string.Empty;

        var fileName = background.Split(',')[2].Replace("\"", string.Empty);

        return File.Exists(Path.Combine(path, fileName))
            ? Path.Combine(path, fileName)
            : string.Empty;
    }

    public string GetArtist()
    {
        if (!OsuPlayerConfig.UseSongnameUnicode) return Artist;
    
        return !string.IsNullOrWhiteSpace(ArtistUnicode) ? ArtistUnicode : Artist;
    }
    
    public string GetTitle()
    {
        if (!OsuPlayerConfig.UseSongnameUnicode) return Title;
    
        return !string.IsNullOrWhiteSpace(TitleUnicode) ? TitleUnicode : Title;
    }
    
    public string GetSongName()
    {
        if (!OsuPlayerConfig.UseSongnameUnicode) return $"{Artist} - {Title}";
    
        if (!string.IsNullOrWhiteSpace(ArtistUnicode) && !string.IsNullOrWhiteSpace(TitleUnicode))
            return $"{ArtistUnicode} - {TitleUnicode}";
    
        return $"{Artist} - {Title}";
    }

    public override string ToString()
    {
        return GetSongName();
    }
}