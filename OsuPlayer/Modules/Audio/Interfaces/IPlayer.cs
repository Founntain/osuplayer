using System.Threading.Tasks;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO.DbReader.Interfaces;
using OsuPlayer.IO.Importer;

namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface represents an audio player which should use the <see cref="IAudioEngine" /> as an underlying base.
/// </summary>
public interface IPlayer : ICommonFeatures, IPlayModes, IHasPlaylists, IHasBlacklist, IHasDiscordRpc
{
    public ISongSourceProvider SongSourceProvider { get; }
    public Bindable<IMapEntry?> CurrentSong { get; }
    public Bindable<string?> CurrentSongImage { get; }
    public int CurrentIndex { get; }

    /// <summary>
    /// Toggles mute of the volume
    /// </summary>
    public void ToggleMute();

    /// <summary>
    /// Plays the next song in the list.
    /// </summary>
    /// <param name="playDirection">The <see cref="PlayDirection" /> for the next song</param>
    public void NextSong(PlayDirection playDirection);

    /// <summary>
    /// Enqueues a specific song to play
    /// </summary>
    /// <param name="song">The song to enqueue</param>
    /// <param name="playDirection">The direction we went in the playlist. Mostly used by the Next and Prev method</param>
    public Task TryPlaySongAsync(IMapEntryBase? song, PlayDirection playDirection = PlayDirection.Normal);
}