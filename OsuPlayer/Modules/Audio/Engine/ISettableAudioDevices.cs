using ManagedBass;
using OsuPlayer.Data.OsuPlayer.Classes;

namespace OsuPlayer.Modules.Audio.Engine;

public interface ISettableAudioDevices
{
    public List<AudioDevice> AvailableAudioDevices { get; }
    public void SetDevice(AudioDevice audioDevice);
}