using OsuPlayer.Data.API.Models.Statistic;

namespace OsuPlayer.Network.API.ApiEndpoints;

public partial class ApiAsync
{
    /// <summary>
    /// Gets API statistics from /api/statistics/getApiStatisticsV2
    /// </summary>
    /// <returns>Statistics as an <see cref="ApiStatisticsV3Model" /></returns>
    public static async Task<ApiStatisticsV3Model> GetApiStatistics()
    {
        // default is new value due to possible null
        return await GetRequestAsync<ApiStatisticsV3Model>("statistics", "getApiStatisticsV2") ?? new ApiStatisticsV3Model();
    }

    /// <summary>
    /// Gets the amount of storage used on the database
    /// </summary>
    /// <returns>Disk space used (in MB) as a float</returns>
    public static async Task<float> GetStorageAmount()
    {
        return await GetRequestAsync<float>("osuplayer", "getStorageAmount");
    }
}