using System.Net;
using System.Text;
using Nein.Extensions;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Services;
using Splat;

namespace OsuPlayer.Network;

public class WebRequestBase : IWebRequest
{
    private readonly ILoggingService _loggingService;
    private readonly IJsonService _jsonService;
    protected string BaseUrl;
    protected readonly CancellationTokenSource CancellationTokenSource = new();

    protected WebRequestBase() :this(string.Empty)
    {
    }

    public WebRequestBase(string baseUrl)
    {
        BaseUrl = baseUrl;

        _loggingService = Locator.Current.GetService<ILoggingService>();
        _jsonService = Locator.Current.GetRequiredService<IJsonService>();
    }

    protected void ParseWebException(Exception ex, Uri url)
    {
        if (ex.GetType() != typeof(WebException)) return;

        var webEx = (WebException) ex;

        _loggingService.Log($"Error while requesting {url}: {webEx.Message}", LogType.Error, webEx);
    }

    public virtual async Task<TResponse> GetRequest<TResponse, TRequest>(string route, TRequest? data = default)
    {
        var url = new Uri($"{BaseUrl}{route}");

        _loggingService.Log($"Request => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Get, url);

            if ( data != null )
                req.Content = new StringContent(await _jsonService.SerializeToJsonStringAsync(data), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var respString = await result.Content.ReadAsStringAsync();

            var response = await _jsonService.DeserializeAsync<TResponse>(respString);

            return response;
        }
        catch (Exception ex)
        {
            ParseWebException(ex, url);

            return default;
        }
    }

    public async Task<HttpResponseMessage> GetRequestWithHttpResponseMessage(string route)
    {
        var url = new Uri($"{BaseUrl}{route}");

        _loggingService.Log($"Request => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Get, url);

            return await client.SendAsync(req, CancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            ParseWebException(ex, url);

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public virtual async Task<TResponse> PostRequest<TResponse, TRequest>(string route, TRequest? data = default)
    {
        var url = new Uri($"{BaseUrl}{route}");

        _loggingService.Log($"Request => {url}");

        try
        {
            using var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            if(data != null)
            {
                if (data is FormUrlEncodedContent content)
                    req.Content = content;
                else
                    req.Content = new StringContent(await _jsonService.SerializeToJsonStringAsync(data), Encoding.UTF8, "application/json");
            }

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = await _jsonService.DeserializeAsync<TResponse>(await result.Content.ReadAsStringAsync());

            return response;
        }
        catch (Exception ex)
        {
            ParseWebException(ex, url);

            return default;
        }
    }
}