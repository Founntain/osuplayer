using OsuPlayer.Data.DataModels;
using OsuPlayer.Interfaces.Service;
using Splat;

namespace OsuPlayer.Services;

public class ProfileManagerService : IProfileManagerService
{
    public User? User { get; set; }

    public async Task Login(string username, string password)
    {
        var result = await Locator.Current.GetService<IOsuPlayerApiService>().LoginAndSaveAuthToken(username, password);

        // If login failed, we set the user to its default value
        if (result == default)
        {
            User = default;

            return;
        }

        User = new User(result);
    }

    public async Task Login(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return;

        var result = await Locator.Current.GetService<IOsuPlayerApiService>().LoginWithTokenAndSaveNewToken(token);

        // If login via token failed, we set the User to its default value
        if (result == default)
        {
            User = default;

            return;
        }

        User = new User(result);
    }
}