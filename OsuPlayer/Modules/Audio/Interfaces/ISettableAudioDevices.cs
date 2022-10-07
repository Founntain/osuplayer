using OsuPlayer.Data.OsuPlayer.Classes;

namespace OsuPlayer.Modules.Audio.Interfaces;

public interface ISettableAudioDevices
{
    public List<AudioDevice> AvailableAudioDevices { get; }
    public void SetDevice(AudioDevice audioDevice);
}