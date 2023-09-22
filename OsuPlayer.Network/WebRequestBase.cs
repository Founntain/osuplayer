using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace OsuPlayer.Network;

public class WebRequestBase : IWebRequest
{
    protected string BaseUrl;
    protected CancellationTokenSource CancellationTokenSource = new();

    public WebRequestBase()
    {
        BaseUrl = string.Empty;
    }

    public WebRequestBase(string baseUrl)
    {
        BaseUrl = baseUrl;
    }
    
    protected void ParseWebException(Exception ex)
    {
        if (ex.GetType() != typeof(WebException)) return;

        var webEx = (WebException) ex;

        if (webEx.Status != WebExceptionStatus.ConnectFailure && webEx.Status != WebExceptionStatus.Timeout) return;
    }

    public virtual async Task<TResponse> GetRequest<TResponse, TRequest>(string route, TRequest? data = default)
    {
        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{BaseUrl}{route}");

            var req = new HttpRequestMessage(HttpMethod.Get, url);

            if ( data != null )
                req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            // CancelCancellationToken();

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var respString = await result.Content.ReadAsStringAsync();
            
            var response = JsonConvert.DeserializeObject<TResponse>(respString);

            return response;
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }

    public async Task<HttpResponseMessage> GetRequestWithHttpResponseMessage(string route)
    {
        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{BaseUrl}{route}");

            var req = new HttpRequestMessage(HttpMethod.Get, url);
            
            return await client.SendAsync(req, CancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public virtual async Task<TResponse> PostRequest<TResponse, TRequest>(string route, TRequest? data = default)
    {
        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{BaseUrl}{route}");

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            if(data != null)
            {
                if (data is FormUrlEncodedContent content)
                    req.Content = content;
                else
                    req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            }

            // CancelCancellationToken();

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var response = JsonConvert.DeserializeObject<TResponse>(await result.Content.ReadAsStringAsync());

            return response;
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }
}