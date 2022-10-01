using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Threading;
using DynamicData;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Playlists;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlaylistViewModel : BaseViewModel
{
    private readonly IObservable<Func<IMapEntryBase, bool>> _filter;
    public readonly Player Player;

    private IDisposable _currentBind;
    private ReadOnlyObservableCollection<IMapEntryBase>? _filteredSongEntries;
    private string _filterText;
    private ObservableCollection<Playlist> _playlists;

    public ObservableCollection<Playlist> Playlists
    {
        get => _playlists;
        set => this.RaiseAndSetIfChanged(ref _playlists, value);
    }

    public Playlist? SelectedPlaylist
    {
        get => Player.SelectedPlaylist.Value;
        set
        {
            PlaylistManager.SetCurrentPlaylist(value);

            Player.SelectedPlaylist.Value = value;

            if (_filteredSongEntries != null)
                _currentBind.Dispose();

            if (Player.SelectedPlaylist.Value?.Songs != null)
                _currentBind = Player.GetMapEntriesFromHash(Player.SelectedPlaylist.Value.Songs).ToSourceList().Connect()
                    .Filter(_filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
                    .Bind(out _filteredSongEntries).Subscribe();

            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(FilteredSongEntries));
        }
    }

    public ReadOnlyObservableCollection<IMapEntryBase>? FilteredSongEntries => _filteredSongEntries;

    public string FilterText
    {
        get => _filterText;
        set => this.RaiseAndSetIfChanged(ref _filterText, value);
    }

    public PlaylistViewModel(Player player)
    {
        Activator = new ViewModelActivator();

        Player = player;

        _filter = this.WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(20))
            .Select(BuildFilter);

        Player.PlaylistChanged += (sender, args) =>
        {
            var selection = Player.SelectedPlaylist.Value;

            if (selection == default)
                return;

            Playlists = PlaylistManager.GetAllPlaylists()?.OrderBy(x => x.Name).ToObservableCollection() ?? new ObservableCollection<Playlist>();

            this.RaisePropertyChanged(nameof(Playlists));

            Player.SelectedPlaylist.Value = Playlists.First(x => x.Id == selection!.Id);

            this.RaisePropertyChanged(nameof(SelectedPlaylist));
        };

        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            Playlists = PlaylistManager.GetAllPlaylists()?.OrderBy(x => x.Name).ToObservableCollection() ?? new ObservableCollection<Playlist>();

            if (Playlists.Count > 0 && SelectedPlaylist == default)
            {
                var playlistStorage = new PlaylistStorage();
                SelectedPlaylist = Playlists.FirstOrDefault(x => x.Id == playlistStorage.Container.LastSelectedPlaylist) ?? Playlists[0];

                PlaylistManager.SetCurrentPlaylist(SelectedPlaylist);
            }
        });
    }

    /// <summary>
    /// Builds the filter to search songs from the song's <see cref="SourceList{T}" />
    /// </summary>
    /// <param name="searchText">the search text to search songs for</param>
    /// <returns>a function with input <see cref="IMapEntryBase" /> and output <see cref="bool" /> to select found songs</returns>
    private Func<IMapEntryBase, bool> BuildFilter(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return _ => true;

        var searchQs = searchText.Split(' ');

        return song => searchQs.All(x =>
            song.Title.Contains(x, StringComparison.OrdinalIgnoreCase) ||
            song.Artist.Contains(x, StringComparison.OrdinalIgnoreCase));
    }
}