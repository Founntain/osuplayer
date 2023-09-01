using OsuPlayer.Api.Data.API.Models;

namespace OsuPlayer.Network.API.Service.NorthFox.Endpoints;

public class NorthFoxApiStatisticsEndpoint : AbstractApiBase
{
    #region GET Requests

    public async Task<ApiStatisticsModel> GetApiStatistics()
    {
        return await GetRequestAsync<ApiStatisticsModel>("ApiStatistics", "get");
    }

    public async Task<double> GetStorageAmount()
    {
        return await GetRequestAsync<double>("ApiStatistics", "getStorageAmount");
    }

    #endregion
}