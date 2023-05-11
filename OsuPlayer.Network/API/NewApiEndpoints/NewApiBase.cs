using System.Net;
using System.Text;
using Newtonsoft.Json;
using OsuPlayer.Api.Data.API;

namespace OsuPlayer.Network.API.NewApiEndpoints;

public static partial class NewApiBase
{
    private static CancellationTokenSource? _cancellationTokenSource;

    private static string Url => Constants.Localhost
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

    private static void CancelCancellationToken()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Creates a GET request to the osu!player API returning T.
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

            CancelCancellationToken();

            var data = await client.GetByteArrayAsync(new Uri($"{Url}{controller}/{action}"), _cancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<T>>(Encoding.UTF8.GetString(data));

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

            CancelCancellationToken();

            var result = await client.SendAsync(req, _cancellationTokenSource.Token);

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

            CancelCancellationToken();

            var data = await client.GetByteArrayAsync(new Uri($"{Url}{controller}/{action}?{parameters}"), _cancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<ApiResponse<T>>(Encoding.UTF8.GetString(data));

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
    public static async Task<T?> ApiRequestWithParametersAsync<T>(string controller, string action, string parameters)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{Url}{controller}/{action}?{parameters}");

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            CancelCancellationToken();

            var result = await client.SendAsync(req, _cancellationTokenSource.Token);

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
}