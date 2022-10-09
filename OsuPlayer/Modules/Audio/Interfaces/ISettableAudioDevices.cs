using OsuPlayer.Data.OsuPlayer.Classes;

namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface provides changeable audio device capability.
/// </summary>
public interface ISettableAudioDevices
{
    public List<AudioDevice> AvailableAudioDevices { get; }

    /// <summary>
    /// Set the audio device to use as the output.
    /// </summary>
    /// <param name="audioDevice">The <see cref="AudioDevice"/> to use</param>
    public void SetDevice(AudioDevice audioDevice);
}