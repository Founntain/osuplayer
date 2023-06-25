using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace OsuPlayer.Network;

public abstract class AbstractRequestBase : IWebRequest
{
    public string BaseUrl;
    protected CancellationTokenSource _cancellationTokenSource = new();
    
    protected void ParseWebException(Exception ex)
    {
        if (ex.GetType() != typeof(WebException)) return;

        var webEx = (WebException) ex;

        if (webEx.Status != WebExceptionStatus.ConnectFailure && webEx.Status != WebExceptionStatus.Timeout) return;
    }

    public async Task<TResponse> GetRequest<TResponse, TRequest>(string route, TRequest? data = default)
    {
        try
        {
            using var client = new HttpClient();

            var url = new Uri($"{BaseUrl}{route}");

            var req = new HttpRequestMessage(HttpMethod.Get, url);

            if ( data != null )
                req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            // CancelCancellationToken();

            var result = await client.SendAsync(req, _cancellationTokenSource.Token);

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

    public async Task<TResponse> PostRequest<TResponse, TRequest>(string route, TRequest? data = default)
    {
        try
        {
            using var client = new HttpClient();

            BaseUrl = $"http://ws.audioscrobbler.com/2.0/";
            
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

            var result = await client.SendAsync(req, _cancellationTokenSource.Token);

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