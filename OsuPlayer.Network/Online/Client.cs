using OsuPlayer.Data.API.Models.User;

namespace OsuPlayerPlus.Classes.Online;

public sealed class Client : ClientModel
{
    //TODO: REIMPLEMENT
    // public SolidColorBrush IsHostBrush => OsuPlayer.PartyManager.Party.Hostname == Username
    //     ? new SolidColorBrush(OsuPlayer.Config.Theme.AccentColor)
    //     : new SolidColorBrush(Colors.White);

    public override string ToString()
    {
        return Username;
    }
}