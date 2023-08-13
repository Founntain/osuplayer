using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Network.API.Service.Endpoints;

public class NorthFoxBadgeEndpoint : AbstractApiBase
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

    #region POST Requests

    // public async Task<UserModel?> EditUser(UserModel editData)
    // {
    //     return await PostRequestAsync<UserModel>("edit", editData);
    // }
    //
    // public async Task<bool> ChangePassword(byte[] newPassword)
    // {
    //     return await PostRequestAsync<bool>("changePassword", newPassword);
    // }
    //
    // public async Task<UserModel?> UpdateSongsPlayed(string username, int amount, int beatmapSetId = -1)
    // {
    //     if (string.IsNullOrWhiteSpace(username)) return default;
    //
    //     return await PostRequestWithParametersAsync<UserModel>("updateSongsPlayed", $"username={username}&amount={amount}&beatmapSetId={beatmapSetId}");
    // }
    //
    // public async Task<UserModel?> UpdateXp(UpdateXpModel updateXpModel)
    // {
    //     return await PostRequestAsync<UserModel>("updateXp", updateXpModel);
    // }

    #endregion

    #region DELETE Requests

    // public async Task<bool> DeleteUser()
    // {
    //     return await DeleteRequestAsync<bool>("delete");
    // }

    #endregion
}