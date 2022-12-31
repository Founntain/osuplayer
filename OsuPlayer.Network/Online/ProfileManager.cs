using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Network.API.Service.Endpoints;
using Splat;

namespace OsuPlayer.Network.Online;

public class ProfileManager
{
    public static UserModel? User = User ?? new ();

    public static async Task Login(string username, string password)
    {
        User = await Locator.Current.GetService<NorthFox>().LoginAndSaveAuthToken(username, password);
    }
}