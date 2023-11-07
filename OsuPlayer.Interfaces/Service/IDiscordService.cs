using DiscordRPC;

namespace OsuPlayer.Interfaces.Service;

public interface IDiscordService
{
    public void Initialize();
    public void DeInitialize();
    public Task UpdatePresence(string details, string state, int beatmapSetId = 0, Assets? assets = null, TimeSpan? durationLeft = null);
}