using System.ComponentModel;

namespace OsuPlayer.Modules.Audio;

public interface IHasBlacklist
{
    public Bindable<bool> BlacklistSkip { get; }

    public event PropertyChangedEventHandler? BlacklistChanged;

    /// <summary>
    /// Triggers if the blacklist got changed
    /// </summary>
    /// <param name="e"></param>
    public void TriggerBlacklistChanged(PropertyChangedEventArgs e);
}