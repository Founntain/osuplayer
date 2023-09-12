using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Network.API.Service.NorthFox.Endpoints;

public class NorthFoxActivityEndpoint
{
    private readonly AbstractApiBase _apiBase;
    
    public NorthFoxActivityEndpoint(AbstractApiBase apiBase)
    {
        _apiBase = apiBase;
    }
    
    #region GET Requests

    public async Task<List<ActivityModel>?> GetAllActivities()
    {
        return await _apiBase.Get<ActivityModel>("Activity");
    }

    public async Task<ActivityModel> GetActivity(Guid uniqueId)
    {
        return await _apiBase.GetById<ActivityModel>("Activity", uniqueId);
    }

    #endregion
}