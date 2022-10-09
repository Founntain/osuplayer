namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface provides discord rpc capabilities.
/// </summary>
public interface IHasDiscordRpc
{
    /// <summary>
    /// Disposes the discord client.
    /// </summary>
    public void DisposeDiscordClient();
}