using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.Enums;

namespace OsuPlayer.Network.Online;

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