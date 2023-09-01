using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Network.API.Service.NorthFox.Endpoints;

public class NorthFoxEventEndpoint : AbstractApiBase
{
    #region GET Requests

    public async Task<List<ActivityModel>?> GetAllEvents()
    {
        return await Get<ActivityModel>("Event");
    }

    public async Task<EventModel> GetEvent(Guid uniqueId)
    {
        return await GetById<EventModel>("Event", uniqueId);
    }

    #endregion
}