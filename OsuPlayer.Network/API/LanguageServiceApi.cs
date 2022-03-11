// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IO;
// using System.Net;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using Api = OsuPlayer.Modules.Network.API.ApiEndpoints.ApiAsync;
//
// namespace OsuPlayer.Modules.Network.API;
//
// public static class LanguageServiceApi
// {
//     public static IEnumerable<ApiLanguage> GetAvailableLanguagesAsync()
//     {
//         using (var wc = new WebClient())
//         {
//             var data = wc.DownloadData(new Uri($"{Api.Url}languages/getAllLanguages"));
//
//             return JsonConvert.DeserializeObject<IEnumerable<ApiLanguage>>(Encoding.UTF8.GetString(data));
//         }
//     }
//
//     public static async Task<string> UploadLanguageToServerAsync(string data)
//     {
//         try
//         {
//             var req = (HttpWebRequest) WebRequest.Create($"{Api.Url}languages/uploadNewLanguage/");
//
//             req.ContentType = "application/json";
//             req.Method = "POST";
//
//             var b = Encoding.UTF8.GetBytes(data);
//             req.ContentLength = b.Length;
//
//             using (var dataStream = await req.GetRequestStreamAsync())
//             {
//                 dataStream.Write(b, 0, b.Length);
//
//                 using (var response = await req.GetResponseAsync())
//                 {
//                     using (var dataStream2 = response.GetResponseStream())
//                     {
//                         if (dataStream2 != null)
//                             using (var reader = new StreamReader(dataStream2))
//                             {
//                                 var responseFromServer = await reader.ReadToEndAsync();
//
//                                 Debug.WriteLine($@"Response: {responseFromServer}");
//
//                                 return responseFromServer;
//                             }
//                     }
//                 }
//             }
//         }
//         catch (Exception)
//         {
//             return string.Empty;
//         }
//
//         return string.Empty;
//     }
//
//     public static ApiLanguage DownloadLanguage(string language)
//     {
//         try
//         {
//             using (var wc = new WebClient())
//             {
//                 return JsonConvert.DeserializeObject<ApiLanguage>(
//                     Encoding.UTF8.GetString(
//                         wc.DownloadData($"{Api.Url}languages/downloadLanguage?language={language}")));
//             }
//         }
//         catch (Exception)
//         {
//             return null;
//         }
//     }
// }