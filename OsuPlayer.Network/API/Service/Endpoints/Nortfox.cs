using System.Text;
using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Network.API.Service.Endpoints;

/// <summary>
/// NorthFox is the wrapper for the OsuPlayer.API and provides access to all public available API Endpoints!
/// Yes I like foxes :)
/// </summary>
public partial class NorthFox
{
    public async Task<UserModel?> LoginAndSaveAuthToken(string username, string password)
    {
        var response = await Login(username, password);

        UserAuthToken = response?.Token;
        
        await File.WriteAllBytesAsync("data/session.op", Encoding.UTF8.GetBytes(UserAuthToken ?? string.Empty));

        return response?.User;
    }
    
    public async Task<UserModel?> LoginWithTokenAndSaveNewToken(string token)
    {
        var response = await Login(token);

        UserAuthToken = response?.Token;
        
        await File.WriteAllBytesAsync("data/session.op", Encoding.UTF8.GetBytes(UserAuthToken ?? string.Empty));

        return response?.User;
    }
}