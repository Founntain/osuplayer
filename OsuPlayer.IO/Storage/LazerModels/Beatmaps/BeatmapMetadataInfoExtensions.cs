namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public static class BeatmapMetadataInfoExtensions
{
    /// <summary>
    /// A user-presentable display title representing this metadata.
    /// </summary>
    public static string GetDisplayTitle(this IBeatmapMetadataInfo metadataInfo)
    {
        string author = string.IsNullOrEmpty(metadataInfo.Author.Username) ? string.Empty : $" ({metadataInfo.Author.Username})";

        string artist = string.IsNullOrEmpty(metadataInfo.Artist) ? "unknown artist" : metadataInfo.Artist;
        string title = string.IsNullOrEmpty(metadataInfo.Title) ? "unknown title" : metadataInfo.Title;

        return $"{artist} - {title}{author}".Trim();
    }
}