namespace OsuPlayer.Modules.Audio;

public interface IHasEffects
{
    public BindableArray<decimal> EqGains { get; }
    public bool IsEqEnabled { get; set; }

    public void SetPlaybackSpeed(double speed);
}