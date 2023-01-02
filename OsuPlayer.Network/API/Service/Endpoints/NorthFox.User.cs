using System.Text;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using OsuPlayer.Api.Data.API;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.User;
using OsuPlayer.Api.Data.API.RequestModels.User.Responses;

namespace OsuPlayer.Network.API.Service.Endpoints;

public partial class NorthFox : AbstractApiBase
{
    #region GET Requests

    /// <summary>
    /// Get the profile picture of a specific user
    /// </summary>
    /// <param name="uniqueId">The ID of the user</param>
    /// <returns>The profile picture as Base64 string</returns>
    public async Task<byte[]?> GetProfilePictureAsync(Guid uniqueId)
    {
        if (uniqueId == Guid.Empty) return default;
        
        // We have to use an own implementation, than the base methods. Reason is that this action doesn't return an Api Response!
        if (Constants.OfflineMode)
            return default;

        try
        {
            using var client = new HttpClient();

            var data = await client.GetByteArrayAsync(new Uri($"{Url}User/getProfilePicture?id={uniqueId}"));

            return data;
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
        if (string.IsNullOrWhiteSpace(bannerUrl)) return default;

        using var client = new HttpClient();

        try
        {
            var data = await client.GetByteArrayAsync(bannerUrl);

            await using var stream = new MemoryStream(data);

            return new Bitmap(stream);
        }
        catch (Exception ex)
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
        return await GetRequestWithParameterAsync<List<UserActivityModel>>("User", "getUserActivityOfUser", $"uniqueId={uniqueId}");
    }
    
    #endregion

    #region POST Requests

    public async Task<UserModel?> Register(AddUserModel data)
    {
        return await PostRequestAsync<UserModel>("User", "register", data);
    }

    public async Task<UserTokenResponse?> Login(string username, string password)
    {
        if (Constants.OfflineMode)
            return default;
        
        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{Url}User/login");

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = GetAuthorizationHeader(username, password);

            CancelCancellationToken();
            
            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<UserTokenResponse>>(await result.Content.ReadAsStringAsync());
            
            return response.Errors?.Any() == true 
                ? default 
                : response.Value;
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }
    
    public async Task<UserTokenResponse?> Login(string token)
    {
        if (Constants.OfflineMode)
            return default;
        
        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{Url}User/loginWithToken");

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            
            req.Headers.Add("session-token", token);

            CancelCancellationToken();
            
            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<UserTokenResponse>>(await result.Content.ReadAsStringAsync());
            
            return response.Errors?.Any() == true 
                ? default 
                : response.Value;
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
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

    #region DELETE Requests

    public async Task<bool> DeleteUser()
    {
        return await DeleteRequestAsync<bool>("User", "delete");
    }

    #endregion
}