using System.ComponentModel;

namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface provides blacklist capability.
/// </summary>
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