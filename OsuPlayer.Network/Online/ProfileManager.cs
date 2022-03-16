using OsuPlayer.Network.API.ApiEndpoints;

namespace OsuPlayer.Network.Online;

public static class ProfileManager
{
    public static User? User;

    public static async Task<User?> LoadProfile(string username, string password)
    {
        User = await ApiAsync.LoadUserWithCredentialsAsync(username, password);

        return User;
    }
}