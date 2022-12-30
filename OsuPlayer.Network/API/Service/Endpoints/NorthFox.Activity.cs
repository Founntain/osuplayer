using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Network.API.Service.Endpoints;

public partial class NorthFox
{
    #region GET Requests
    
    public async Task<List<ActivityModel>?> GetAllActivities()
    {
        return await Get<ActivityModel>("Activity");
    }

    public async Task<ActivityModel> GetActivity(Guid uniqueId)
    {
        return await GetById<ActivityModel>("Activity", uniqueId);
    }
    
    #endregion
}