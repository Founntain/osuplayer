using System.Text;
using Newtonsoft.Json;
using OsuPlayer.Api.Data.API;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.User.Responses;

namespace OsuPlayer.Network.API.Service.Endpoints;

/// <summary>
/// NorthFox is the wrapper for the OsuPlayer.API and provides access to all public available API Endpoints!
/// Yes I like foxes :)
/// </summary>
public class NorthFox : AbstractApiBase
{
    public NorthFoxActivityEndpoint Activity { get; set; }
    public NorthFoxApiStatisticsEndpoint ApiStatistics { get; set; }
    public NorthFoxBadgeEndpoint Badge { get; set; }
    public NorthFoxBeatmapEndpoint Beatmap { get; set; }
    public NorthFoxEventEndpoint Event { get; set; }
    public NorthFoxUserEndpoint User { get; set; }
    public NorthFoxUserStatisticsEndpoint UserStatistics { get; set; }
    
    public async Task<UserModel?> LoginAndSaveAuthToken(string username, string password)
    {
        var response = await Login(username, password);

        UserAuthToken = response?.Token;

        await File.WriteAllBytesAsync("data/session.op", Encoding.UTF8.GetBytes(UserAuthToken ?? string.Empty));

        return response?.User;
    }

    public async Task<UserModel?> LoginWithTokenAndSaveNewToken(string token)
    {
        var response = await Login(token);

        UserAuthToken = response?.Token;

        await File.WriteAllBytesAsync("data/session.op", Encoding.UTF8.GetBytes(UserAuthToken ?? string.Empty));

        return response?.User;
    }

    #region Authentication

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

            // CancelCancellationToken();

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

            // CancelCancellationToken();

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

    #endregion
}