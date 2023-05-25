using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Network.API.Service.Endpoints;

public partial class NorthFox
{
    #region GET Requests

    public async Task<List<BadgeModel>?> GetAllBadges()
    {
        return await Get<BadgeModel>("Badge");
    }

    public async Task<BadgeModel> GetBadge(Guid uniqueId)
    {
        return await GetById<BadgeModel>("Badge", uniqueId);
    }

    #endregion
}