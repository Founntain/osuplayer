using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Octokit;
using OsuPlayer.Api.Data.API;

namespace OsuPlayer.Network.API.Service;

public abstract class AbstractApiBase
{
    protected string Url => Constants.Localhost
        ? "https://localhost:7096/"
        : "https://osuplayer.founntain.dev/api/";
    
    protected static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
    
    protected string? UserAuthToken { get; set; }
    
    protected void ParseWebException(Exception ex)
    {
        if (ex.GetType() != typeof(WebException)) return;

        var webEx = (WebException) ex;

        if (webEx.Status != WebExceptionStatus.ConnectFailure && webEx.Status != WebExceptionStatus.Timeout) return;
        if (Constants.OfflineMode) return;

        Constants.OfflineMode = true;
    }

    /// <summary>
    /// Cancels the current CancellationToken (if one exists) and creates a new one!
    /// </summary>
    protected void CancelCancellationToken()
    {
        CancellationTokenSource.Cancel();
        CancellationTokenSource = new ();
    }

    protected AuthenticationHeaderValue GetAuthorizationHeader(string username, string password)
    {
        var authenticationString = $"{username}:{password}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

        return new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
    }
    
    #region GET Requests

    /// <summary>
    /// Calls the default Get Method of the controller
    /// </summary>
    /// <typeparam name="T">The Type that is expected to return</typeparam>
    /// <returns>An <see cref="List{T}"/></returns>
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
    /// <returns>an object of type <see cref="T"/></returns>
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

        try
        {
            using var client = new HttpClient();
            
            var url = new Uri($"{Url}{controller}/{action}");
            
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("session-token", UserAuthToken);
            
            CancelCancellationToken();
            
            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response =  JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());
            
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

        try
        {
            using var client = new HttpClient();
            
            var url = new Uri($"{Url}{controller}/{action}?{parameters}");
            
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("session-token", UserAuthToken);
            
            CancelCancellationToken();
            
            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response =  JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());
            
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
        
        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{Url}{controller}/{action}");

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("session-token", UserAuthToken);
            req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            CancelCancellationToken();
            
            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response =  JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());
            
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

        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{Url}{controller}/{action}?{parameters}");

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("session-token", UserAuthToken);

            CancelCancellationToken();

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());
            
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
        
        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{Url}{controller}/{action}");

            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            req.Headers.Add("session-token", UserAuthToken);
            req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            CancelCancellationToken();
            
            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response =  JsonConvert.DeserializeObject<ApiResponse<T>>(await result.Content.ReadAsStringAsync());
            
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