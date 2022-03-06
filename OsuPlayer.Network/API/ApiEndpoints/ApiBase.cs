using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OsuPlayer.Network;

namespace OsuPlayerPlus.Classes.API.ApiEndpoints;

public static partial class ApiAsync
{
    public static string Url => Constants.Localhost 
        ? "http://localhost:5000/api/" 
        : "https://osuplayer.founntain.dev/api/";

    private static void OfflineModeMessage()
    {
        //TODO: IMPLEMENT NEW MESSAGE BOX
        // OsuPlayerMessageBox.Show(
        //     "osu!player is not able to connect to the API in any way. We enabled offline mode to give you a better experience, however you still will get updates. Check your connection and disable offline mode in the settings. If the server is not reachable you need to wait for us to fix it.");
    }

    private static void ParseWebException(Exception ex)
    {
        if (ex.GetType() != typeof(WebException)) return;

        var webEx = (WebException) ex;

        if (webEx.Status != WebExceptionStatus.ConnectFailure && webEx.Status != WebExceptionStatus.Timeout) return;
        if (Constants.OfflineMode) return;

        OfflineModeMessage();
        Constants.OfflineMode = true;
    }

    /// <summary>
    ///     Creates a GET request to the osu!player API return <see cref="T" />
    /// </summary>
    /// <param name="call">The route of the controller</param>
    /// <param name="controller">The controller to call</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns an object of type T</returns>
    public static async Task<T> GetRequestAsync<T>(string call, string controller)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using (var wc = new HttpClient())
            {
                var data = await wc.GetByteArrayAsync(new Uri($"{Url}{controller}/{call}"));

                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
            }
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }

    /// <summary>
    ///     Creates a POST request to the osu!player API returning <see cref="T" />.
    /// </summary>
    /// <param name="call">The route of the controller</param>
    /// <param name="controller">The controller to call</param>
    /// <param name="data">Date to send</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns an object of type T</returns>
    public static async Task<T?> ApiRequestAsync<T>(string call, string controller, object data = null)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("Content-Type", "application/json");
                wc.Encoding = Encoding.UTF8;

                var url = new Uri($"{Url}{controller}/{call}");
                var result = await wc.UploadStringTaskAsync(url, "POST", JsonConvert.SerializeObject(data));

                return JsonConvert.DeserializeObject<T>(result);
            }
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }

    public static async Task<T?> GetRequestWithIdAsync<T>(string call, string controller, string id)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using (var wc = new HttpClient())
            {
                var data = await wc.GetByteArrayAsync(new Uri($"{Url}{controller}/{call}?id={id}"));

                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
            }
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }

    public static async Task<T?> GetRequestWithNameAsync<T>(string call, string controller, string name)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using (var wc = new HttpClient())
            {
                var data = await wc.GetByteArrayAsync(new Uri($"{Url}{controller}/{call}?name={name}"));
                
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
            }
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return default;
        }
    }
}