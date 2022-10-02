using System.ComponentModel;

namespace OsuPlayer.Modules.Audio.Engine;

public interface IAudioEngine : ICanOpenFiles, IAudioControls, IHasEffects
{
    public event PropertyChangedEventHandler? PropertyChanged;
}