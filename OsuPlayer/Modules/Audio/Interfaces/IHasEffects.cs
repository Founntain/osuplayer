namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface provides audio effects capability.
/// </summary>
public interface IHasEffects
{
    public BindableArray<decimal> EqGains { get; }
    public bool IsEqEnabled { get; set; }

    /// <summary>
    /// Sets the playback speed (including pitch)
    /// </summary>
    /// <param name="speed">The speed to set</param>
    public void SetPlaybackSpeed(double speed);

    /// <summary>
    /// Triggers a Method change. Usually this should only be called when toggling pitch or other FX settings
    /// </summary>
    public void UpdatePlaybackMethod();
}