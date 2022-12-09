using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace OsuPlayer.Network.API.ApiEndpoints;

/// <summary>
/// Static class to make HTTP Requests to the osu!player api asynchronously
/// </summary>
public static partial class ApiAsync
{
    public static string Url => Constants.Localhost
        ? "http://localhost:5000/api/"
        : "https://osuplayer.founntain.dev/api/";

    private static void ParseWebException(Exception ex)
    {
        if (ex.GetType() != typeof(WebException)) return;

        var webEx = (WebException) ex;

        if (webEx.Status != WebExceptionStatus.ConnectFailure && webEx.Status != WebExceptionStatus.Timeout) return;
        if (Constants.OfflineMode) return;

        Constants.OfflineMode = true;
    }

    /// <summary>
    /// Creates a GET request to the osu!player API return T.
    /// </summary>
    /// <param name="controller">The controller to call</param>
    /// <param name="action">The route of the controller</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns an object of type T</returns>
    public static async Task<T?> GetRequestAsync<T>(string controller, string action)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using var client = new HttpClient();
            
            var data = await client.GetByteArrayAsync(new Uri($"{Url}{controller}/{action}"));

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
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
    /// <param name="data">Date to send</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns an object of type T</returns>
    public static async Task<T?> ApiRequestAsync<T>(string controller, string action, object? data = null)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using var client = new HttpClient();
            
            var url = new Uri($"{Url}{controller}/{action}");

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(req);

            return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
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
    public static async Task<T?> GetRequestWithParameterAsync<T>(string controller, string action, string parameters)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using var client = new HttpClient();
            
            var data = await client.GetByteArrayAsync(new Uri($"{Url}{controller}/{action}?{parameters}"));

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
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
    public static async Task<T?> ApiRequestWithParametersAsync<T>(string controller, string action, string parameters)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using var client = new HttpClient();
            
            var url = new Uri($"{Url}{controller}/{action}?{parameters}");

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            var result = await client.SendAsync(req);

            return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }
}