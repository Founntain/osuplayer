using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.API.Models.User;

namespace OsuPlayerPlus.Classes.Online;

public sealed class OnlineUserStatusModelExtended : UserOnlineStatusModel
{
    public string Status => GetStatusDisplay();

    public string GetStatusDisplay()
    {
        switch (StatusType)
        {
            case UserOnlineStatusType.Listening:
                return $"Listening to {Song}";
            case UserOnlineStatusType.InParty:
                return $"Dancing in a party to {Song}";
            default:
                return "Idle";
        }
    }
}