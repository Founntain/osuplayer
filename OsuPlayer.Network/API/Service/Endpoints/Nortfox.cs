namespace OsuPlayer.Network.API.Service.Endpoints;

public partial class NorthFox
{
    public async Task GetUserAuthToken(string username, string password)
    {
        var response = await Login(username, password);

        UserAuthToken = response?.Token;
    }
}