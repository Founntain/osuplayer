using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Modules.Audio.Interfaces;

public interface IPlayModes
{
    public Bindable<bool> IsShuffle { get; }
    public Bindable<RepeatMode> RepeatMode { get; }
}