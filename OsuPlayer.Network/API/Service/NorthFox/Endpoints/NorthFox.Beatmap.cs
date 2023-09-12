using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.Beatmap;
using OsuPlayer.Api.Data.API.RequestModels.Statistics;
using OsuPlayer.Api.Data.API.ResponseModels;

namespace OsuPlayer.Network.API.Service.NorthFox.Endpoints;

public class NorthFoxBeatmapEndpoint
{
    private readonly AbstractApiBase _apiBase;
    
    public NorthFoxBeatmapEndpoint(AbstractApiBase apiBase)
    {
        _apiBase = apiBase;
    }
    
    #region POST Requests

    public async Task<List<AddBeatmapModel>> AddBeatmap(List<AddBeatmapModel> beatmapsToAdd)
    {
        return await _apiBase.PostRequestAsync<List<AddBeatmapModel>>("Beatmap", "add", beatmapsToAdd);
    }

    #endregion

    #region GET Requests

    public async Task<List<BeatmapModel>?> GetAllBeatmaps()
    {
        return await _apiBase.Get<BeatmapModel>("Beatmap");
    }

    public async Task<BeatmapModel> GetBeatmap(Guid uniqueId)
    {
        return await _apiBase.GetById<BeatmapModel>("Beatmap", uniqueId);
    }

    public async Task<BeatmapSearchResponse> GetBeatmapsPaged(SearchBeatmapModel model)
    {
        return await _apiBase.PostRequestAsync<BeatmapSearchResponse>("Beatmap", "getBeatmaps", model);
    }

    public async Task<UserStatsModel?> GetBeatmapsPlayedByUser(Guid uniqueId, int amount = 0)
    {
        return await _apiBase.GetRequestWithParameterAsync<UserStatsModel>("Beatmap", "beatmapsPlayedByUser", $"id={uniqueId}&amount={amount}");
    }

    #endregion
}