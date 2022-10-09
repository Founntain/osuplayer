namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface wraps all sub-interfaces used by the audio engine.
/// </summary>
public interface IAudioEngine : ICanOpenFiles, ICommonFeatures
{
    public delegate void ChannelReachedEndHandler();

    public Bindable<double> ChannelLength { get; }
    public Bindable<double> ChannelPosition { get; }
    public ChannelReachedEndHandler ChannelReachedEnd { set; }
}