using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.Statistics;

namespace OsuPlayer.Network.API.NorthFox;

public class NorthFoxUserStatisticsEndpoint
{
    private readonly AbstractApiBase _apiBase;
    
    public NorthFoxUserStatisticsEndpoint(AbstractApiBase apiBase)
    {
        _apiBase = apiBase;
    }
    
    #region POST Requests

    public async Task<bool> AddUserStatistic(PostUserStatisticModel data)
    {
        return await _apiBase.PostRequestAsync<bool>("UserStatistics", "add", data);
    }

    #endregion

    #region GET Requests

    public async Task<List<UserStatisticModel>?> GetAllUserStatistics()
    {
        return await _apiBase.Get<UserStatisticModel>("UserStatistics");
    }

    public async Task<UserStatisticModel> GetAllUserStatistics(Guid uniqueId)
    {
        return await _apiBase.GetById<UserStatisticModel>("UserStatistics", uniqueId);
    }

    #endregion
}