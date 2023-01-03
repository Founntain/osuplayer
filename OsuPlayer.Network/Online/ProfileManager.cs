using OsuPlayer.Network.API.Service.Endpoints;
using Splat;

namespace OsuPlayer.Network.Online;

public class ProfileManager
{
    public static User? User = User ?? new ();

    public static async Task Login(string username, string password)
    {
        var result = await Locator.Current.GetService<NorthFox>().LoginAndSaveAuthToken(username, password);

        // If login failed, we set the user to its default value
        if (result == default)
        {
            User = default;
            
            return;
        }

        User = new User(result);
    }

    public static async Task Login(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return;
        
        var result = await Locator.Current.GetService<NorthFox>().LoginWithTokenAndSaveNewToken(token);

        // If login via token failed, we set the User to its default value
        if (result == default)
        {
            User = default;
            
            return;
        }

        User = new User(result);
    }
}