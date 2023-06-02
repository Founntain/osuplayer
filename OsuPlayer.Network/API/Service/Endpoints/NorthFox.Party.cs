using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.Party;

namespace OsuPlayer.Network.API.Service.Endpoints;

public partial class NorthFox
{
    #region GET Requests

    public async Task<List<PartyModel>?> GetAllParties()
    {
        return await Get<PartyModel>("Party");
    }

    public async Task<PartyModel> GetParty(Guid uniqueId)
    {
        return await GetById<PartyModel>("Party", uniqueId);
    }

    #endregion

    #region POST Requests

    public async Task<PartyModel?> CreateParty(CreatePartyModel createPartyModel)
    {
        return await PostRequestAsync<PartyModel?>("Party", "add", createPartyModel);
    }

    #endregion

    #region DELETE Requests

    #endregion
}