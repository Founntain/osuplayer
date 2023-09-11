using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.Beatmap;
using OsuPlayer.Api.Data.API.RequestModels.Statistics;
using OsuPlayer.Api.Data.API.ResponseModels;

namespace OsuPlayer.Network.API.Service.NorthFox.Endpoints;

public class NorthFoxBeatmapEndpoint : AbstractApiBase
{
    #region POST Requests

    public async Task<List<AddBeatmapModel>> AddBeatmap(List<AddBeatmapModel> beatmapsToAdd)
    {
        return await PostRequestAsync<List<AddBeatmapModel>>("Beatmap", "add", beatmapsToAdd);
    }

    #endregion

    #region GET Requests

    public async Task<List<BeatmapModel>?> GetAllBeatmaps()
    {
        return await Get<BeatmapModel>("Beatmap");
    }

    public async Task<BeatmapModel> GetBeatmap(Guid uniqueId)
    {
        return await GetById<BeatmapModel>("Beatmap", uniqueId);
    }

    public async Task<BeatmapSearchResponse> GetBeatmapsPaged(SearchBeatmapModel model)
    {
        return await PostRequestAsync<BeatmapSearchResponse>("Beatmap", "getBeatmaps", model);
    }

    public async Task<UserStatsModel?> GetBeatmapsPlayedByUser(Guid uniqueId, int amount = 0)
    {
        return await GetRequestWithParameterAsync<UserStatsModel>("Beatmap", "beatmapsPlayedByUser", $"id={uniqueId}&amount={amount}");
    }

    #endregion
}