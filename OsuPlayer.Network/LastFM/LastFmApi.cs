using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Nein.Extensions;
using Newtonsoft.Json;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.IO.Storage.LazerModels.Extensions;
using OsuPlayer.Network.LastFM.Responses;
using OsuPlayer.Network.LastFM.Responses.Scrobble;

namespace OsuPlayer.Network.LastFM;

public class LastFmApi : AbstractRequestBase
{
    /// <summary>
    /// The Last.FM API-Key of the user
    /// </summary>
    private string _apiKey;

    private string _authToken;

    /// <summary>
    /// The Last.FM API-Secret of the user
    /// </summary>
    private string _secret;

    /// <summary>
    /// The session key has an infinite lifespan, the key gets invalid when the user revokes permissions to this application
    /// </summary>
    private string? _sessionKey;

    public LastFmApi()
    {
        BaseUrl = "http://ws.audioscrobbler.com/2.0/?format=json";

        using var config = new Config();

        _apiKey = config.Container.LastFmApiKey;
        _secret = config.Container.LastFmSecret;
        _authToken = string.Empty;
    }

    public LastFmApi(string apiKey, string secret)
    {
        _apiKey = apiKey;
        _secret = secret;
        _authToken = string.Empty;

        BaseUrl = "http://ws.audioscrobbler.com/2.0/?format=json";
    }

    public void SetApiKeyAndSecret(string apiKey, string secret)
    {
        _apiKey = apiKey;
        _secret = secret;
    }

    public async Task Scrobble(string title, string artist)
    {
        if (string.IsNullOrWhiteSpace(_sessionKey))
        {
            Console.WriteLine("can't scrobble to the last.fm api, because there isn't a valid session key");
            return;
        }

        var parameters = new Dictionary<string, string>();

        parameters.Add("track", title);
        parameters.Add("artist", artist);
        parameters.Add("timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

        var apiSignature = GetApiSignature("track.scrobble", parameters);

        parameters.Add("api_sig", apiSignature);

        var response = await PostRequest<ScrobblesResponse, object>(string.Empty, new FormUrlEncodedContent(parameters));

        Debug.WriteLine($"SCROBBLING | {JsonConvert.ToString(response)}");
    }

    public async Task SaveSessionKeyAsync()
    {
        await File.WriteAllBytesAsync("data/lastfm.session", Encoding.UTF8.GetBytes(_sessionKey ?? string.Empty));

        using var config = new Config();

        config.Container.LastFmApiKey = _apiKey;
        config.Container.LastFmSecret = _secret;
    }

    public bool LoadSessionKey()
    {
        if (!File.Exists("data/lastfm.session")) return false;

        var sessionBytes = File.ReadAllBytes("data/lastfm.session");

        _sessionKey = Encoding.UTF8.GetString(sessionBytes);

        return !string.IsNullOrWhiteSpace(_sessionKey);
    }


    public async Task<bool> LoadSessionKeyAsync()
    {
        if (!File.Exists("data/lastfm.session")) return false;

        var sessionBytes = await File.ReadAllBytesAsync("data/lastfm.session");

        _sessionKey = Encoding.UTF8.GetString(sessionBytes);

        return !string.IsNullOrWhiteSpace(_sessionKey);
    }

    public bool IsAuthorized()
    {
        return !string.IsNullOrWhiteSpace(_sessionKey);
    }

    #region Last.FM Authorization

    public async Task<string> GetAuthToken()
    {
        var response = await GetRequest<TokenResponse, object>("&method=auth.gettoken");

        _authToken = response.Token;

        return _authToken;
    }

    public void AuthorizeToken()
    {
        GeneralExtensions.OpenUrl($"http://www.last.fm/api/auth/?api_key={_apiKey}&token={_authToken}");
    }

    public async Task GetSessionKey()
    {
        var parameters = new Dictionary<string, string>();

        parameters.Add("token", _authToken);

        var signature = $"&api_sig={GetApiSignature("auth.getSession", parameters)}";

        var response = await SessionRequest($"&method=auth.getSession&token={_authToken}{signature}");

        if (response == null) return;

        _sessionKey = response.Key;
    }

    private async Task<SessionResponse?> SessionRequest(string route)
    {
        try
        {
            using var client = new HttpClient();

            var baseUrl = $"http://ws.audioscrobbler.com/2.0/?api_key={_apiKey}";

            var url = new Uri($"{baseUrl}{route}");

            var req = new HttpRequestMessage(HttpMethod.Get, url);

            // CancelCancellationToken();

            var result = await client.SendAsync(req, CancellationTokenSource.Token);

            var respString = await result.Content.ReadAsStringAsync();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(respString);
            var xpath = "lfm/session/key";

            var nodes = xmlDoc.SelectNodes(xpath);

            return nodes?.Count > 0 ? new SessionResponse(null, nodes[0]!.ChildNodes[0]!.Value!, 0) : null;
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }

    #endregion

    #region Helper Functions

    private Dictionary<string, string> GetBaseParameters()
    {
        var dict = new Dictionary<string, string>();

        dict.Add("api_key", _apiKey);

        if (!string.IsNullOrWhiteSpace(_sessionKey))
            dict.Add("sk", _sessionKey);

        return dict;
    }

    private string GetApiSignature(string method, Dictionary<string, string>? parameters = null)
    {
        parameters ??= new Dictionary<string, string>();

        parameters.AddRange(GetBaseParameters());

        parameters.Add("method", method);

        var sortedParams = parameters.OrderBy(p => p.Key);

        // Step 3: Concatenate parameter names and values
        var paramString = string.Concat(sortedParams.Select(p => p.Key + p.Value));

        // Step 4: Append API secret
        var paramWithSecret = paramString + _secret;

        using var md5 = MD5.Create();

        var paramBytes = Encoding.UTF8.GetBytes(paramWithSecret);
        var hashBytes = md5.ComputeHash(paramBytes);
        var apiSig = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return apiSig;
    }

    #endregion
}