using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using OsuPlayer.Api.Data.API;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Services;
using Splat;

namespace OsuPlayer.Network.API;

public abstract class AbstractApiBase
{
    protected internal abstract string ApiName { get; }

    protected internal readonly ILoggingService loggingService;
    private readonly IProfileManagerService _profileManager;

    protected static CancellationTokenSource CancellationTokenSource = new();

    protected internal string Url => GetApiUrl();

    protected string? UserAuthToken { get; set; }

    protected AbstractApiBase()
    {
        loggingService = Locator.Current.GetService<ILoggingService>();
        _profileManager = Locator.Current.GetService<IProfileManagerService>();

        loggingService.Log($"{ApiName} uses the following base URL: {Url}");
    }

    private string GetApiUrl()
    {
        var url = "https://osuplayer.founntain.dev/";

        bool.TryParse(Environment.GetEnvironmentVariable("USE_LOCAL_API"), out var useLocalApi);
        bool.TryParse(Environment.GetEnvironmentVariable("USE_SANDBOX_API"), out var useSandboxApi);

        if (useLocalApi)
        {
            url = "https://localhost:7096/";
        }

        if (useSandboxApi)
        {
            url = "https://sandbox.founntain.dev/";
        }

        return url;
    }

    protected internal void ParseWebException(Exception ex, Uri url)
    {
        if (ex.GetType() != typeof(WebException)) return;

        var webEx = (WebException) ex;

        loggingService.Log($"Error while requesting {url}: {webEx.Message}", LogType.Error, webEx);

        if (webEx.Status != WebExceptionStatus.ConnectFailure && webEx.Status != WebExceptionStatus.Timeout) return;
        if (Constants.OfflineMode) return;

        Constants.OfflineMode = true;
    }

    /// <summary>
    /// Cancels the current CancellationToken (if one exists) and creates a new one!
    /// </summary>
    // protected void CancelCancellationToken()
    // {
    //     // TODO: Currently does nothing, because it breaks the functionality a bit.
    //     // TODO: This needs to be re-thinked a bit!
    //     return;
    //
    //     CancellationTokenSource.Cancel();
    //     CancellationTokenSource = new ();
    // }
    protected AuthenticationHeaderValue GetAuthorizationHeader(string username, string password)
    {
        var authenticationString = $"{username}:{password}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

        return new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
    }

    #region DELETE Requests

    /// <summary>
    /// Creates a DELETE request to the osu!player API returning T.
    /// </summary>
    /// ///
    /// <param name="controller">The controller to call</param>
    /// <param name="action">The route of the controller</param>
    /// <param name="data">Date to send</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns an object of type T</returns>
    public async Task<T?> DeleteRequestAsync<T>(string controller, string action, object? data = null)
    {
        if (Constants.OfflineMode)
            return default;

        var url = new Uri($"{Url}{controller}/{action}");

        loggingService.Log($"Requesting => {ApiName} => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Delete, url);

            req.Headers.Add("username", _profileManager.User?.Name);
            req.Headers.Add("session-token", UserAuthToken);

            req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            // CancelCancellationToken();

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());

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

    #region GET Requests

    /// <summary>
    /// Calls the default Get Method of the controller
    /// </summary>
    /// <typeparam name="T">The Type that is expected to return</typeparam>
    /// <returns>An <see cref="List{T}" /></returns>
    public async Task<List<T>?> Get<T>(string controller)
    {
        return await GetRequestAsync<List<T>>(controller, "get");
    }

    /// <summary>
    /// calls the default Get Method of a single object via its unique ID
    /// </summary>
    /// <param name="controller">The controller to call</param>
    /// <param name="uniqueId">The unique ID of the object</param>
    /// <typeparam name="T">The type that is expected to return</typeparam>
    /// <returns>an object of type <see cref="T" /></returns>
    public async Task<T?> GetById<T>(string controller, Guid uniqueId)
    {
        return await GetRequestWithParameterAsync<T>(controller, "getById", $"id={uniqueId}");
    }

    /// <summary>
    /// Creates a GET request to the osu!player API returning T.
    /// </summary>
    /// <param name="controller">The controller to call</param>
    /// <param name="action">The route of the controller</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>An object of type T</returns>
    public async Task<T?> GetRequestAsync<T>(string controller, string action)
    {
        if (Constants.OfflineMode)
            return default;

        var url = new Uri($"{Url}{controller}/{action}");

        loggingService.Log($"Requesting => {ApiName} => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Get, url);

            req.Headers.Add("username", _profileManager.User?.Name);
            req.Headers.Add("session-token", UserAuthToken);

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());

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

    /// <summary>
    /// Creates a GET request to the osu!player api with parameters returning T.
    /// </summary>
    /// <param name="controller">The controller to call</param>
    /// <param name="action">The route of the controller</param>
    /// <param name="parameters">Paramaters for the call</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns an object of type T</returns>
    public async Task<T?> GetRequestWithParameterAsync<T>(string controller, string action, string parameters)
    {
        if (Constants.OfflineMode)
            return default;

        var url = new Uri($"{Url}{controller}/{action}?{parameters}");

        loggingService.Log($"Requesting => {ApiName} => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Get, url);

            req.Headers.Add("username", _profileManager.User?.Name);
            req.Headers.Add("session-token", UserAuthToken);

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());

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

    #region POST Requests

    /// <summary>
    /// Creates a POST request to the osu!player API returning T.
    /// </summary>
    /// ///
    /// <param name="controller">The controller to call</param>
    /// <param name="action">The route of the controller</param>
    /// <param name="data">Date to send</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns an object of type T</returns>
    public async Task<T?> PostRequestAsync<T>(string controller, string action, object? data = null)
    {
        if (Constants.OfflineMode)
            return default;

        var url = new Uri($"{Url}{controller}/{action}");

        loggingService.Log($"Requesting => {ApiName} => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            req.Headers.Add("username", _profileManager.User?.Name);
            req.Headers.Add("session-token", UserAuthToken);

            req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());

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

    /// <summary>
    /// Creates a POST request to the osu!player API returning T.
    /// </summary>
    /// ///
    /// <param name="controller">The controller to call</param>
    /// <param name="action">The route of the controller</param>
    /// <param name="parameters">Parameters</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns an object of type T</returns>
    public async Task<T?> PostRequestWithParametersAsync<T>(string controller, string action, string parameters)
    {
        if (Constants.OfflineMode)
            return default;

        var url = new Uri($"{Url}{controller}/{action}?{parameters}");

        loggingService.Log($"Requesting => {ApiName} => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            req.Headers.Add("username", _profileManager.User?.Name);
            req.Headers.Add("session-token", UserAuthToken);

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());

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