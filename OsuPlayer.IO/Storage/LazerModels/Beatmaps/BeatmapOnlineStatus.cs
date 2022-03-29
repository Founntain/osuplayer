namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public enum BeatmapOnlineStatus
{
    None = -3,

    Graveyard = -2,

    WIP = -1,

    Pending = 0,

    Ranked = 1,

    Approved = 2,

    Qualified = 3,

    Loved = 4
}

public static class BeatmapSetOnlineStatusExtensions
{
    public static bool GrantsPerformancePoints(this BeatmapOnlineStatus status)
    {
        return status == BeatmapOnlineStatus.Ranked || status == BeatmapOnlineStatus.Approved;
    }
}