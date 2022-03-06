//TODO: IMPLEMENT WHEN NEEDED

// using System;
// using System.Net;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using OsuPlayer.Data.API.Enums;
// using OsuPlayer.Data.API.Models.User;
// using OsuPlayer.Network;
// using OsuPlayerPlus.Classes.Online;
//
// namespace OsuPlayerPlus.Classes.API.ApiEndpoints;
//
// public static partial class ApiAsync
// {
//     public static async Task<string> GetProfilePictureAsync(string name)
//     {
//         if (Constants.OfflineMode)
//             return null;
//
//         if (name == null) return null;
//
//         try
//         {
//             using (var wc = new WebClient())
//             {
//                 var data = await wc.DownloadDataTaskAsync(new Uri($"{Url}users/getProfilePictureByName?name={name}"));
//
//                 return Encoding.UTF8.GetString(data);
//             }
//         }
//         catch (Exception ex)
//         {
//             ParseWebException(ex);
//
//             return null;
//         }
//     }
//
//     public static async Task<User> GetProfileAsync(string id)
//     {
//         if (Constants.OfflineMode)
//             return null;
//
//         try
//         {
//             using (var wc = new WebClient())
//             {
//                 var data = await wc.DownloadDataTaskAsync($"{Url}users/getUserByGUID?id={id}");
//
//                 return JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(data));
//             }
//         }
//         catch (Exception ex)
//         {
//             ParseWebException(ex);
//
//             return null;
//         }
//     }
//
//     public static async Task<User?> GetProfileByNameAsync(string name)
//     {
//         if (Constants.OfflineMode)
//             return null;
//
//         try
//         {
//             using (var wc = new WebClient())
//             {
//                 var data = await wc.DownloadDataTaskAsync(new Uri($"{Url}users/getUserByName?name={name}"));
//
//                 return JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(data));
//             }
//         }
//         catch (Exception ex)
//         {
//             ParseWebException(ex);
//
//             return null;
//         }
//     }
//
//     public static async Task<User?> UpdateXpFromCurrentUserAsync(string songChecksum, double elapsedMilliseconds,
//         double channelLength)
//     {
//         if (Constants.OfflineMode)
//             return null;
//
//         if (OsuPlayer.Profile.User == null || string.IsNullOrWhiteSpace(OsuPlayer.Profile.User.Name))
//             return null;
//
//         var updateXpModel = new UpdateXpModel
//         {
//             Username = OsuPlayer.Profile.User.Name,
//             SongChecksum = songChecksum,
//             ElapsedMilliseconds = elapsedMilliseconds,
//             ChannelLength = channelLength
//         };
//
//         return await ApiRequestAsync<User>("updateXp", "users", updateXpModel);
//     }
//
//     public static async Task<User?> UpdateSongsPlayedCurrentUserAsync(int amount, int beatmapSetId = -1)
//     {
//         if (Constants.OfflineMode)
//             return null;
//
//         if (OsuPlayer.Profile.User == null || string.IsNullOrWhiteSpace(OsuPlayer.Profile.User.Name))
//             return null;
//
//         try
//         {
//             using (var wc = new WebClient())
//             {
//                 var data = await wc.DownloadStringTaskAsync(new Uri(
//                     $"{Url}users/updateSongsPlayed?username={OsuPlayer.Profile.User.Name}&amount={amount}&beatmapSetId={beatmapSetId}"));
//
//                 return JsonConvert.DeserializeObject<User>(data);
//             }
//         }
//         catch (Exception ex)
//         {
//             ParseWebException(ex);
//
//             return null;
//         }
//     }
//
//     public static async Task<UserOnlineStatusModel?> UpdateUserOnlineStatusAsync(UserOnlineStatusType statusType,
//         string song = null, string songChecksum = null)
//     {
//         var username = OsuPlayer.Profile?.User?.Name;
//
//         if (string.IsNullOrWhiteSpace(username)) return default;
//
//         return await ApiRequestAsync<UserOnlineStatusModel>("setUserOnlineStatus", "users", new UserOnlineStatusModel
//         {
//             Username = username,
//             Song = song,
//             SongChecksum = songChecksum,
//             StatusType = statusType
//         });
//     }
//
//     public static async Task<T> GetAllOnlineStatusModelAsync<T>()
//     {
//         return await GetRequestAsync<T>("getAllOnlineStatuses", "users");
//     }
//
//     public static async Task<bool> SetPasswordForUserAsync(string username, string password)
//     {
//         return await ApiRequestAsync<bool>("setPasswordForUser", "users", new UserPasswordModel
//         {
//             Username = username,
//             Password = password
//         });
//     }
//
//     public static async Task<bool> CheckPasswordForUserAsync(string username, string password)
//     {
//         return await ApiRequestAsync<bool>("checkPasswordForUser", "users", new UserPasswordModel
//         {
//             Username = username,
//             Password = password
//         });
//     }
//
//     public static async Task<User?> LoadUserWithCredentialsAsync(string username, string password)
//     {
//         return await ApiRequestAsync<User>("loadUserWithCredentials", "users", new UserPasswordModel
//         {
//             Username = username,
//             Password = password
//         });
//     }
//
//     public static async Task<bool> HasUserPasswordSetAsync(string username)
//     {
//         return await GetRequestWithNameAsync<bool>("hasUserPasswordSet", "users", username);
//     }
// }