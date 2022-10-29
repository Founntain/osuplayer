namespace OsuPlayer.Network.API.ApiEndpoints;

public partial class ApiAsync
{
    /// <summary>
    /// Gets beatmap count from /api/beatmaps/getBeatmapsCount
    /// </summary>
    /// <returns>Beatmap count as an integer</returns>
    public static async Task<int> GetBeatmapsCount()
    {
        return await GetRequestAsync<int>("beatmaps", "getBeatmapsCount");
    }
}