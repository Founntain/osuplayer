using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO.Importer;

namespace OsuPlayer.Modules.Audio;

public interface IPlayer : ICanImportSongs, ISortableSongs, IPlayState, IHasPlaylists, IHasBlacklist, IHasEffects, IHasStatistics
{
    public Bindable<IMapEntry> CurrentSong { get; }
    public Bindable<Bitmap?> CurrentSongImage { get; }
    public int CurrentIndex { get; set; }

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
    /// Toggles mute of the volume
    /// </summary>
    public void ToggleMute();

    /// <summary>
    /// Plays the previous song or the last song if we are the beginning
    /// </summary>
    public void PreviousSong();
    
    /// <summary>
    /// Plays the next song in the list. Or the first if we are at the end
    /// </summary>
    public void NextSong();
    
    /// <summary>
    /// Enqueues a specific song to play
    /// </summary>
    /// <param name="song">The song to enqueue</param>
    /// <param name="playDirection">The direction we went in the playlist. Mostly used by the Next and Prev method</param>
    public Task TryPlaySongAsync(IMapEntryBase? song, PlayDirection playDirection = PlayDirection.Normal);

    /// <summary>
    /// Gets the map entry from the beatmap hash
    /// </summary>
    /// <param name="hash">the beatmap hash to get the map from</param>
    /// <returns>the found <see cref="IMapEntryBase" /> or null if it doesn't exist</returns>
    public IMapEntryBase? GetMapEntryFromHash(string hash);

    /// <summary>
    /// Gets all Songs from a specific beatmap hash
    /// </summary>
    /// <param name="hash">The beatmap hash</param>
    /// <returns>a list of <see cref="IMapEntryBase" />s</returns>
    public List<IMapEntryBase> GetMapEntriesFromHash(IEnumerable<string> hash);
}

public interface IPlayState
{
    public Bindable<bool> IsPlaying { get; }
    public Bindable<bool> IsShuffle { get; }
    public Bindable<RepeatMode> RepeatMode { get; }
}