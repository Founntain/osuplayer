using System.Threading.Tasks;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.Party;
using OsuPlayer.Network.API.Service.Endpoints;
using Splat;

namespace OsuPlayer.Modules.Party;

public class PartyManager
{
    public PartyModel CurrentParty;

    public async Task<PartyModel> CreateParty()
    {
        var model = new CreatePartyModel
        {

        };
        
        return await Locator.Current.GetService<NorthFox>().CreateParty(model);
    }
}