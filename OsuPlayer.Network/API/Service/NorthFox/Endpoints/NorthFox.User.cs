﻿using Avalonia.Media.Imaging;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.User;

namespace OsuPlayer.Network.API.Service.NorthFox.Endpoints;

public class NorthFoxUserEndpoint : AbstractApiBase
{
    #region DELETE Requests

    public async Task<bool> DeleteUser()
    {
        return await DeleteRequestAsync<bool>("User", "delete");
    }

    #endregion

    #region GET Requests

    /// <summary>
    /// Get the profile picture of a specific user
    /// </summary>
    /// <param name="uniqueId">The ID of the user</param>
    /// <returns>The profile picture as Base64 string</returns>
    public async Task<Bitmap?> GetProfilePictureAsync(Guid uniqueId)
    {
        if (uniqueId == Guid.Empty)
            return default;

        // We have to use an own implementation, than the base methods. Reason is that this action doesn't return an ApiResponse<T>
        if (Constants.OfflineMode)
            return default;

        try
        {
            using var client = new HttpClient();

            var data = await client.GetAsync(new Uri($"{Url}User/getProfilePicture?id={uniqueId}"));

            return new Bitmap(await data.Content.ReadAsStreamAsync());
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }

    /// <summary>
    /// Gets the banner of a specific user
    /// </summary>
    /// <param name="bannerUrl">The url of the banner</param>
    /// <returns>The banner as an <see cref="Bitmap" /></returns>
    public async Task<Bitmap?> GetProfileBannerAsync(string? bannerUrl = null)
    {
        if (string.IsNullOrWhiteSpace(bannerUrl))
            return default;

        using var client = new HttpClient();

        try
        {
            var data = await client.GetAsync(bannerUrl);

            return new Bitmap(await data.Content.ReadAsStreamAsync());
        }
        catch (Exception)
        {
            return default;
        }
    }

    public async Task<List<UserModel>?> GetAllUsers()
    {
        return await Get<UserModel>("User");
    }

    public async Task<UserModel> GetUser(Guid uniqueId)
    {
        return await GetById<UserModel>("User", uniqueId);
    }

    public async Task<UserModel?> GetUserFromLoginToken()
    {
        return await GetRequestAsync<UserModel>("User", "getUserFromLoginToken");
    }

    public async Task<List<UserActivityModel>?> GetActivityOfUser(Guid uniqueId)
    {
        return await GetRequestWithParameterAsync<List<UserActivityModel>>("User", "getActivityOfUser", $"uniqueId={uniqueId}");
    }

    #endregion

    #region POST Requests

    public async Task<UserModel?> Register(AddUserModel data)
    {
        return await PostRequestAsync<UserModel>("User", "register", data);
    }

    public async Task<UserModel?> EditUser(EditUserModel editData)
    {
        return await PostRequestAsync<UserModel>("User", "edit", editData);
    }

    public async Task<bool> ChangePassword(byte[] newPassword)
    {
        return await PostRequestAsync<bool>("User", "changePassword", newPassword);
    }

    public async Task<UserModel?> UpdateSongsPlayed(int amount, int beatmapSetId = -1)
    {
        return await PostRequestWithParametersAsync<UserModel>("User", "updateSongsPlayed", $"&amount={amount}&beatmapSetId={beatmapSetId}");
    }

    public async Task<UserModel?> UpdateXp(UpdateXpModel updateXpModel)
    {
        return await PostRequestAsync<UserModel>("User", "updateXp", updateXpModel);
    }

    public async Task<bool> SaveProfilePicture(byte[] data)
    {
        return await PostRequestAsync<bool>("User", "saveProfilePicture", data);
    }

    public async Task<bool> SetOnlineStatus(UserOnlineStatusModel data)
    {
        return await PostRequestAsync<bool>("User", "setOnlineStatus", data);
    }

    #endregion
}