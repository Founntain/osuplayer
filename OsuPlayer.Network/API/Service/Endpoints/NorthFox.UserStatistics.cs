﻿using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.Statistics;

namespace OsuPlayer.Network.API.Service.Endpoints;

public partial class NorthFox
{
    #region GET Requests
    
    public async Task<List<UserStatisticModel>?> GetAllUserStatistics()
    {
        return await Get<UserStatisticModel>("UserStatistics");
    }

    public async Task<UserStatisticModel> GetAllUserStatistics(Guid uniqueId)
    {
        return await GetById<UserStatisticModel>("UserStatistics", uniqueId);
    }
    
    #endregion

    #region POST Requests

    public async Task<bool> AddUserStatistic(PostUserStatisticModel data)
    {
        return await PostRequestAsync<bool>("UserStatistics", "add", data);
    }

    #endregion
}