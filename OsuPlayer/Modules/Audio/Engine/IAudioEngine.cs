using System.ComponentModel;

namespace OsuPlayer.Modules.Audio.Engine;

public interface IAudioEngine : ICanOpenFiles, IAudioControls, ISettableAudioDevices, IHasEffects
{
    public event PropertyChangedEventHandler? PropertyChanged;
}