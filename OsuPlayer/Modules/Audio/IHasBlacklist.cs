using System.ComponentModel;

namespace OsuPlayer.Modules.Audio;

public interface IHasBlacklist
{
    public Bindable<bool> BlacklistSkip { get; }

    public event PropertyChangedEventHandler? BlacklistChanged;

    public void TriggerBlacklistChanged(PropertyChangedEventArgs e);
}