using System.Text;
using Newtonsoft.Json;
using OsuPlayer.Api.Data.API;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.User.Responses;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Interfaces.Service.Endpoint;

namespace OsuPlayer.Network.API.NorthFox;

/// <summary>
/// NorthFox is the wrapper for the OsuPlayer.API and provides access to all public available API Endpoints!
/// Yes I like foxes 🦊
/// </summary>
public class NorthFox : AbstractApiBase, IOsuPlayerApiService
{
    protected internal override string ApiName => "NorthFox API 🦊";

    #region API Endpoints

    public NorthFoxActivityEndpoint Activity { get; set; }
    public NorthFoxApiStatisticsEndpoint ApiStatistics { get; set; }
    public NorthFoxBadgeEndpoint Badge { get; set; }
    public NorthFoxBeatmapEndpoint Beatmap { get; set; }
    public NorthFoxEventEndpoint Event { get; set; }
    public IOsuPlayerApiUserEndpoint User { get; set; }
    public NorthFoxUserStatisticsEndpoint UserStatistics { get; set; }

    #endregion

    public NorthFox()
    {
        Activity = new NorthFoxActivityEndpoint(this);
        ApiStatistics = new NorthFoxApiStatisticsEndpoint(this);
        Badge = new NorthFoxBadgeEndpoint(this);
        Beatmap = new NorthFoxBeatmapEndpoint(this);
        Event = new NorthFoxEventEndpoint(this);
        User = new NorthFoxUserEndpoint(this);
        UserStatistics = new NorthFoxUserStatisticsEndpoint(this);
    }

    #region Authentication

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

    private async Task<UserTokenResponse?> Login(string username, string password)
    {
        if (Constants.OfflineMode)
            return default;

        var url = new Uri($"{Url}User/login");

        loggingService.Log($"Requesting => {ApiName} => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = GetAuthorizationHeader(username, password);

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<UserTokenResponse>>(await result.Content.ReadAsStringAsync());

            return response.Errors?.Any() == true
                ? default
                : response.Value;
        }
        catch (Exception ex)
        {
            ParseWebException(ex, url);

            return default;
        }
    }

    private async Task<UserTokenResponse?> Login(string token)
    {
        if (Constants.OfflineMode)
            return default;

        var url = new Uri($"{Url}User/loginWithToken");

        loggingService.Log($"Requesting => {ApiName} => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            req.Headers.Add("session-token", token);

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<UserTokenResponse>>(await result.Content.ReadAsStringAsync());

            return response.Errors?.Any() == true
                ? default
                : response.Value;
        }
        catch (Exception ex)
        {
            ParseWebException(ex, url);

            return default;
        }
    }

    #endregion
}