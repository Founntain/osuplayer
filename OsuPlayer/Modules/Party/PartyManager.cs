using System.Threading.Tasks;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.Party;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Network.API.Service.Endpoints;
using Splat;

namespace OsuPlayer.Modules.Party;

public class PartyManager
{
    public Bindable<PartyModel?> CurrentParty = new();

    public async Task<PartyModel?> CreateParty()
    {
        var player = Locator.Current.GetService<IPlayer>();
        
        var model = new CreatePartyModel
        {
            BeatmapHash = player.CurrentSong.Value.Hash,
            FullSongname = player.CurrentSong.Value.Artist + " - " + player.CurrentSong.Value.Title,
            PlaybackSpeed = player.GetPlaybackSpeed(),
            IsPaused = true,
            UsePitch = true
        };
        
        var newParty = await Locator.Current.GetService<NorthFox>().CreateParty(model);

        if (newParty == default) return default;

        CurrentParty.Value = newParty;

        return CurrentParty.Value;
    }
}