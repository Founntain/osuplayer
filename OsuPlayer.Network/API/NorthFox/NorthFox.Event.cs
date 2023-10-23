using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Network.API.NorthFox;

public class NorthFoxEventEndpoint
{
    private readonly AbstractApiBase _apiBase;
    
    public NorthFoxEventEndpoint(AbstractApiBase apiBase)
    {
        _apiBase = apiBase;
    }
    
    #region GET Requests

    public async Task<List<ActivityModel>?> GetAllEvents()
    {
        return await _apiBase.Get<ActivityModel>("Event");
    }

    public async Task<EventModel> GetEvent(Guid uniqueId)
    {
        return await _apiBase.GetById<EventModel>("Event", uniqueId);
    }

    #endregion
}