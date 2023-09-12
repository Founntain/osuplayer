using OsuPlayer.Api.Data.API.Models;

namespace OsuPlayer.Network.API.Service.NorthFox.Endpoints;

public class NorthFoxApiStatisticsEndpoint
{
    private readonly AbstractApiBase _apiBase;
    
    public NorthFoxApiStatisticsEndpoint(AbstractApiBase apiBase)
    {
        _apiBase = apiBase;
    }
    
    #region GET Requests

    public async Task<ApiStatisticsModel?> GetApiStatistics()
    {
        return await _apiBase.GetRequestAsync<ApiStatisticsModel?>("ApiStatistics", "get");
    }

    public async Task<double> GetStorageAmount()
    {
        return await _apiBase.GetRequestAsync<double>("ApiStatistics", "getStorageAmount");
    }

    #endregion
}