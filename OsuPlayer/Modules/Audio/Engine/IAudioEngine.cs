using System.ComponentModel;

namespace OsuPlayer.Modules.Audio.Engine;

public interface IAudioEngine : ICanOpenFiles, ICommonFeatures
{
    public Bindable<double> ChannelLength { get; }
    public Bindable<double> ChannelPosition { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
}