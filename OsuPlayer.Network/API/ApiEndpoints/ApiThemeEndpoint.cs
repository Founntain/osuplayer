//TODO: IMPLEMENTED WHEN ITS NEEDED

// using System;
// using System.Net;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using OsuPlayer.Network;
//
// namespace OsuPlayerPlus.Classes.API.ApiEndpoints;
//
// public static partial class ApiAsync
// {
//     public static async Task<Theme> GetThemeFromUserAsync(string name)
//     {
//         if (Constants.OfflineMode)
//             return null;
//
//         try
//         {
//             using (var wc = new WebClient())
//             {
//                 var data = await wc.DownloadStringTaskAsync(new Uri($"{Url}users/getTheme?name={name}"));
//
//                 return JsonConvert.DeserializeObject<Theme>(data);
//             }
//         }
//         catch (Exception ex)
//         {
//             ParseWebException(ex);
//
//             return null;
//         }
//     }
// }