using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Nein.Extensions;
using OsuPlayer.IO.Storage.LazerModels.Extensions;
using OsuPlayer.Network.LastFM.Responses;

namespace OsuPlayer.Network.LastFM;

public class LastFmApi : AbstractRequestBase
{
    /// <summary>
    /// The Last.FM API-Key of the user
    /// </summary>
    private readonly string _apiKey;
    
    /// <summary>
    /// The Last.FM API-Secret of the usercc
    /// </summary>
    private readonly string _secret;

    private string _authToken;

    /// <summary>
    /// The session key has an infinite lifespan, the key gets invalid when the user revokes permissions to this application
    /// </summary>
    private string? _sessionKey;

    public LastFmApi(string apiKey, string secret)
    {
        _apiKey = apiKey;
        _secret = secret;
        _authToken = string.Empty;
        
        BaseUrl = $"http://ws.audioscrobbler.com/2.0/?format=json&api_key={_apiKey}";
    }
    
    public async Task UpdateNowPlaying(string title, string artist, ulong duration)
    {
        var p = new Dictionary<string, string>();

        p.Add("track", title);
        p.Add("artist", artist);
        // p.Add("duration", duration.ToString());

        var apiSignature = GetApiSignature("track.updateNowPlaying", p, true);
        
        var apiSignatureUrl = $"&api_sig={apiSignature}";

        var x = new
        {
            Api_Key = _apiKey,
            Sk = _sessionKey,
            Track = title,
            Artist = artist,
            Method = "track.updateNowPlaying",
            Api_Sig = apiSignature
        };
        
        await PostRequest<object, object>(string.Empty, new FormUrlEncodedContent(p));
    }
    
    #region Last.FM Authorization

    public async Task<string> GetAuthToken()
    {
        var response = await GetRequest<TokenResponse, object>($"&method=auth.gettoken&format=json");

        _authToken = response.Token;

        return _authToken;
    }

    public void AuthorizeToken()
    {
        GeneralExtensions.OpenUrl($"http://www.last.fm/api/auth/?api_key={_apiKey}&token={_authToken}");
    }

    public async Task GetSessionKey()
    {
        var p = new Dictionary<string, string>();
        
        p.Add("token", _authToken);

        var signature = $"&api_sig={GetApiSignature("auth.getSession", p, true)}";

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

            var result = await client.SendAsync(req, _cancellationTokenSource.Token);

            var respString = await result.Content.ReadAsStringAsync();
            
            XmlDocument xmlDoc = new XmlDocument();
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

    private string GetApiSignature(string method, Dictionary<string, string>? parameters = null, bool excludeJsonParameter = false)
    {
        parameters ??= new Dictionary<string, string>();
        
        parameters.AddRange(GetBaseParameters(excludeJsonParameter));
        
        parameters.Add("method", method);
        
        if(_sessionKey != null)
            parameters.Add("sk", _sessionKey);
        
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
    
    #region Helper Functions

    private Dictionary<string, string> GetBaseParameters(bool excludeJsonParamater = false)
    {
        var dict = new Dictionary<string, string>();
        
        dict.Add("api_key", _apiKey);
        
        if(!excludeJsonParamater)
            dict.Add("format", "json");

        return dict;
    }

    #endregion

    public async Task SaveSessionKey()
    {
        await File.WriteAllBytesAsync("data/lastfm.session", Encoding.UTF8.GetBytes(_sessionKey ?? string.Empty));
    }

    public async Task LoadSessionKey()
    {
        if (!File.Exists("data/lastfm.session")) return;
        
        var sessionBytes = await File.ReadAllBytesAsync("data/lastfm.session");

        _sessionKey = Encoding.UTF8.GetString(sessionBytes);
    }

    public bool IsAuthorized()
    {
        return !string.IsNullOrWhiteSpace(_sessionKey);
    }
}