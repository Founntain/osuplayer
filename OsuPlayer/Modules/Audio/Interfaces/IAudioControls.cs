namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface provides basic audio control functions.
/// </summary>
public interface IAudioControls
{
    public Bindable<bool> IsPlaying { get; }
    public Bindable<double> Volume { get; }

    /// <summary>
    /// Pauses the current song if playing or plays again if paused
    /// </summary>
    public void PlayPause();

    /// <summary>
    /// Sets the playing state to Playing
    /// </summary>
    public void Play();

    /// <summary>
    /// Sets the playing state to Pause
    /// </summary>
    public void Pause();

    /// <summary>
    /// Stops the current song and disposes the audio stream
    /// </summary>
    public void Stop();
}