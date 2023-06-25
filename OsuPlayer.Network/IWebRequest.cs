namespace OsuPlayer.Network;

public interface IWebRequest
{
    public Task<TResponse> GetRequest<TResponse, TRequest>(string route, TRequest? data = default);
    public Task<TResponse> PostRequest<TResponse, TRequest>(string route, TRequest? data = default);
}