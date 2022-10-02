using System.ComponentModel;

namespace OsuPlayer.Modules.Audio.Engine;

public interface IAudioEngine : ICanOpenFiles, ICommonFeatures
{
    public event PropertyChangedEventHandler? PropertyChanged;
}