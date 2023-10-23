using OsuPlayer.Interfaces.Service;

namespace OsuPlayer.Services;

public class LastFmService : OsuPlayerService, ILastFmApiService
{
    private readonly ILastFmApiService _lastFmApiService;

    public override string ServiceName => "LASTFM_SERVICE";

    public LastFmService( ILastFmApiService lastFmApiService )
    {
        _lastFmApiService = lastFmApiService;
    }

    public void AuthorizeToken() => _lastFmApiService.AuthorizeToken();
    public void SetApiKeyAndSecret(string apiKey, string secret) => _lastFmApiService.SetApiKeyAndSecret(apiKey, secret);
    public bool LoadSessionKey() => _lastFmApiService.LoadSessionKey();
    public bool IsAuthorized() => _lastFmApiService.IsAuthorized();
    public Task GetSessionKey() => _lastFmApiService.GetSessionKey();
    public Task SaveSessionKeyAsync() => _lastFmApiService.SaveSessionKeyAsync();
    public Task Scrobble(string title, string artist) => _lastFmApiService.Scrobble(title, artist);
    public Task<bool> LoadSessionKeyAsync() => _lastFmApiService.LoadSessionKeyAsync();
    public Task<string> GetAuthToken() => _lastFmApiService.GetAuthToken();
}