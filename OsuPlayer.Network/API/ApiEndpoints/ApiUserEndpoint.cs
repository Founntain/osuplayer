using Avalonia.Media.Imaging;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.API.Models.Beatmap;
using OsuPlayer.Data.API.Models.User;
using OsuPlayer.Network.Online;

namespace OsuPlayer.Network.API.ApiEndpoints;

public partial class ApiAsync
{
    /// <summary>
    /// Get the profile picture of a specific user
    /// </summary>
    /// <param name="username">The username of the user</param>
    /// <returns>The profile picture as Base64 string</returns>
    public static async Task<string?> GetProfilePictureAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return default;

        return await GetRequestWithParameterAsync<string>("users", "getProfilePictureByName", $"name={username}");
    }

    /// <summary>
    /// Gets the banner of a specific user
    /// </summary>
    /// <param name="bannerUrl">The url of the banner</param>
    /// <returns>The banner as an <see cref="Bitmap" /></returns>
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

    /// <summary>
    /// Gets the whole data of a specific user
    /// </summary>
    /// <param name="username">The username of the user</param>
    /// <returns>A <see cref="User" /> with all its data</returns>
    public static async Task<User?> GetProfileByNameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return default;

        return await GetRequestWithParameterAsync<User>("users", "getUserByName", $"name={username}");
    }

    /// <summary>
    /// Asks the API to update the users XP by calculating it's XP value
    /// </summary>
    /// <param name="songChecksum">The song the user played</param>
    /// <param name="elapsedMilliseconds">How long the user played it</param>
    /// <param name="channelLength">The total song length</param>
    /// <returns>The updated <see cref="User" /></returns>
    public static async Task<User?> UpdateXpFromCurrentUserAsync(string songChecksum, double elapsedMilliseconds,
        double channelLength)
    {
        if (ProfileManager.User?.Name == default) return default;

        var updateXpModel = new UpdateXpModel
        {
            Username = ProfileManager.User.Name,
            SongChecksum = songChecksum,
            ElapsedMilliseconds = elapsedMilliseconds,
            ChannelLength = channelLength
        };

        return await ApiRequestAsync<User>("users", "updateXp", updateXpModel);
    }

    /// <summary>
    /// Updates the songs played statistic of the current user
    /// </summary>
    /// <param name="amount">How many songs the user played</param>
    /// <param name="beatmapSetId">The beatmapset ID to update service side playing statistics</param>
    /// <returns>The updated <see cref="User" /></returns>
    public static async Task<User?> UpdateSongsPlayedForCurrentUserAsync(int amount, int beatmapSetId = -1)
    {
        if (ProfileManager.User?.Name == default) return default;

        return await GetRequestWithParameterAsync<User>("users", "updateSongsPlayed",
            $"username={ProfileManager.User.Name}&amount={amount}&beatmapSetId={beatmapSetId}");
    }

    /// <summary>
    /// Loads the user with given credentials
    /// </summary>
    /// <param name="username">The username of the user</param>
    /// <param name="password">The password of the user</param>
    /// <returns>Returns the user if login was successful</returns>
    public static async Task<User?> LoadUserWithCredentialsAsync(string username, string password)
    {
        return await ApiRequestAsync<User>("users", "loadUserWithCredentials", new UserPasswordModel
        {
            Username = username,
            Password = password
        });
    }

    /// <summary>
    /// Get's the top amound of beatmaps the user played most
    /// </summary>
    /// <param name="username">The username of the user to get it's stats from</param>
    /// <param name="amount">The amount of songs to get. Default 10</param>
    /// <returns>A list of <see cref="BeatmapUserValidityModel" /> containing the top songs of the user</returns>
    public static async Task<List<BeatmapUserValidityModel>?> GetBeatmapsPlayedByUser(string username, int amount = 10)
    {
        return await GetRequestWithParameterAsync<List<BeatmapUserValidityModel>>("beatmaps", "getBeatmapsPlayedByUser",
            $"username={username}&amount={amount}");
    }

    public static async Task<List<(string, int, int)>?> GetActivityOfUser(string username)
    {
        return await GetRequestWithParameterAsync<List<(string, int, int)>>("statistics", "getActivityOfUser", $"username={username}");
    }

    public static async Task<UserOnlineStatusModel?> SetUserOnlineStatus(UserOnlineStatusType statusType, string? song = null, string? checksum = null)
    {
        var username = ProfileManager.User?.Name;

        if (string.IsNullOrWhiteSpace(username))
            return default;

        var data = new UserOnlineStatusModel
        {
            Username = username,
            StatusType = statusType,
            Song = song,
            SongChecksum = checksum
        };

        return await ApiRequestAsync<UserOnlineStatusModel>("users", "setUserOnlineStatus", data);
    }
    
    public static async void SetUserOnlineStatusNonBlock(UserOnlineStatusType statusType, string? song = null, string? checksum = null)
    {
        var username = ProfileManager.User?.Name;

        if (string.IsNullOrWhiteSpace(username))
            return;

        var data = new UserOnlineStatusModel
        {
            Username = username,
            StatusType = statusType,
            Song = song,
            SongChecksum = checksum
        };

        await ApiRequestAsync<UserOnlineStatusModel>("users", "setUserOnlineStatus", data);
    }
}