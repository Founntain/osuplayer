using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OsuPlayer.Data.API.Models.Party;
using OsuPlayer.Network;

namespace OsuPlayerPlus.Classes.API.ApiEndpoints;

public static partial class ApiAsync
{
    public static async Task<PartyModel> GetPartyAsync(string id)
    {
        if (Constants.OfflineMode)
            return default;

        try
        {
            using (var wc = new WebClient())
            {
                var data = await wc.DownloadDataTaskAsync(
                    new Uri($"{Url}parties/getParty?partyID={id}"));

                return JsonConvert.DeserializeObject<PartyModel>(Encoding.UTF8.GetString(data));
            }
        }
        catch (Exception ex)
        {
            ParseWebException(ex);

            return null;
        }
    }
}