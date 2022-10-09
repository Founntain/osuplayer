using System.ComponentModel;
using OsuPlayer.Data.OsuPlayer.StorageModels;

namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface provides playlist capability.
/// </summary>
public interface IHasPlaylists
{
    public Bindable<Playlist?> SelectedPlaylist { get; }
    public Bindable<bool> PlaylistEnableOnPlay { get; }

    public event PropertyChangedEventHandler? PlaylistChanged;

    /// <summary>
    /// Triggers if the playlist got changed
    /// </summary>
    /// <param name="e"></param>
    public void TriggerPlaylistChanged(PropertyChangedEventArgs e);
}