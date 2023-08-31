using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.Beatmap;
using OsuPlayer.Api.Data.API.RequestModels.Statistics;
using OsuPlayer.Network.API.ModelExtensions;

namespace OsuPlayer.Network.API.Service.Endpoints;

public class NorthFoxBeatmapEndpoint : AbstractApiBase
{
    #region POST Requests

    public async Task<List<AddBeatmapModel>> AddBeatmap(List<AddBeatmapModel> beatmapsToAdd)
    {
        return await PostRequestAsync<List<AddBeatmapModel>>("Beatmap", "add", beatmapsToAdd);
    }

    #endregion

    #region GET Requests

    public async Task<List<ApiBeatmapModel>?> GetAllBeatmaps()
    {
        return await Get<ApiBeatmapModel>("Beatmap");
    }

    public async Task<BeatmapModel> GetBeatmap(Guid uniqueId)
    {
        return await GetById<BeatmapModel>("Beatmap", uniqueId);
    }

    public async Task<List<ApiBeatmapModel>> GetBeatmapsPaged(int page = 1, int pageSize = 50)
    {
        return await GetRequestWithParameterAsync<List<ApiBeatmapModel>>("Beatmap", "getPaged", $"page={page}&pageSize={pageSize}");
    }

    public async Task<UserStatsModel?> GetBeatmapsPlayedByUser(Guid uniqueId, int amount = 0)
    {
        return await GetRequestWithParameterAsync<UserStatsModel>("Beatmap", "beatmapsPlayedByUser", $"id={uniqueId}&amount={amount}");
    }

    #endregion
}