namespace OsuPlayer.Modules.Audio.Engine;

public interface IHasEffects
{
    public BindableArray<decimal> EqGains { get; }
    public bool IsEqEnabled { get; set; }

    /// <summary>
    /// Sets the playback speed globally (including pitch)
    /// </summary>
    /// <param name="speed">The speed to set</param>
    public void SetPlaybackSpeed(double speed);
}