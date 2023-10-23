namespace OsuPlayer.Interfaces.Service;

public interface ILastFmApiService
{
    public void SetApiKeyAndSecret(string apiKey, string secret);
    public Task Scrobble(string title, string artist);
    public bool LoadSessionKey();
    public Task<bool> LoadSessionKeyAsync();
    public bool IsAuthorized();
    public Task<string> GetAuthToken();
    public void AuthorizeToken();
    public Task GetSessionKey();
    public Task SaveSessionKeyAsync();
}