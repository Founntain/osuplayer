using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Interfaces.Service.Endpoint;

namespace OsuPlayer.Interfaces.Service;

public interface IOsuPlayerApiService
{
    public IOsuPlayerApiUserEndpoint User { get; set; }

    public Task<UserModel?> LoginAndSaveAuthToken(string username, string password);
    public Task<UserModel?> LoginWithTokenAndSaveNewToken(string token);
}