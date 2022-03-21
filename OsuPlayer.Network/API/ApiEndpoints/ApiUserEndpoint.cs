using Avalonia.Media.Imaging;
using OsuPlayer.Data.API.Models.Beatmap;
using OsuPlayer.Data.API.Models.User;
using OsuPlayer.Network.Online;

namespace OsuPlayer.Network.API.ApiEndpoints;

public partial class ApiAsync
{
    public static async Task<string?> GetProfilePictureAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return default;

        return await GetRequestWithParameterAsync<string>("users", "getProfilePictureByName", $"name={username}");
    }

    public static async Task<Bitmap?> GetProfileBannerAsync(string? bannerUrl = null)
    {
        if (string.IsNullOrWhiteSpace(bannerUrl)) return default;

        using (var client = new HttpClient())
        {
            try
            {
                var data = await client.GetByteArrayAsync(bannerUrl);

                await using (var stream = new MemoryStream(data))
                {
                    return new Bitmap(stream);
                }
            }
            catch (Exception)
            {
                return default;
            }
        }
    }

    public static async Task<User?> GetUserByName(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return default;

        return await GetRequestWithParameterAsync<User>("users", "getUserByName", $"name={username}");
    }

    public static async Task<User?> GetProfileByNameAsync(string username)
    {
        return await GetRequestWithParameterAsync<User>("users", "getUserByName", $"name={username}");
    }

    public static async Task<User?> UpdateXpFromCurrentUserAsync(string songChecksum, double elapsedMilliseconds,
        double channellength)
    {
        var updateXpModel = new UpdateXpModel
        {
            Username = ProfileManager.User.Name,
            SongChecksum = songChecksum,
            ElapsedMilliseconds = elapsedMilliseconds,
            ChannelLength = channellength
        };

        return await ApiRequestAsync<User>("users", "updateXp", updateXpModel);
    }

    public static async Task<User?> UpdateSongsPlayedForCurrentUserAsync(int amount, int beatmapSetId = -1)
    {
        if (string.IsNullOrWhiteSpace(ProfileManager.User?.Name))
            return default;

        return await GetRequestWithParameterAsync<User>("users", "updateSongsPlayed",
            $"amount={amount}&beatmapSetId={beatmapSetId}");
    }

    public static async Task<User?> LoadUserWithCredentialsAsync(string username, string password)
    {
        return await ApiRequestAsync<User>("users", "loadUserWithCredentials", new UserPasswordModel
        {
            Username = username,
            Password = password
        });
    }

    public static async Task<List<BeatmapUserValidityModel>?> GetBeatmapsPlayedByUser(string username, int amount = 10)
    {
        return await GetRequestWithParameterAsync<List<BeatmapUserValidityModel>>("beatmaps", "getBeatmapsPlayedByUser",
            $"username={username}&amount={amount}");
    }
}