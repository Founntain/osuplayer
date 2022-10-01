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

    public void PlayPause();
    public void Play();
    public void Pause();
    public void ToggleMute();

    public void PreviousSong();
    public void NextSong();
    public Task TryPlaySongAsync(IMapEntryBase? song, PlayDirection playDirection = PlayDirection.Normal);

    public IMapEntryBase? GetMapEntryFromHash(string hash);
    public List<IMapEntryBase> GetMapEntriesFromHash(IEnumerable<string> hash);
}

public interface IPlayState
{
    public Bindable<bool> IsPlaying { get; }
    public Bindable<bool> IsShuffle { get; }
    public Bindable<RepeatMode> RepeatMode { get; }
}