using System.Threading.Tasks;

namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface wraps all sub-interfaces used by the audio engine.
/// </summary>
public interface IAudioEngine : ICanOpenFiles, ICommonFeatures
{
    public delegate Task ChannelReachedEndHandler();

    public Bindable<double> ChannelLength { get; }
    public Bindable<double> ChannelPosition { get; }
    public Bindable<double> PlaybackSpeed { get; }
    public ChannelReachedEndHandler ChannelReachedEnd { set; }
}