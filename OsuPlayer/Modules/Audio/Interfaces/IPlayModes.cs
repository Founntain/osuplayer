using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface provides different play modes for the player.
/// </summary>
public interface IPlayModes
{
    public Bindable<bool> IsShuffle { get; }
    public Bindable<RepeatMode> RepeatMode { get; }
}