using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlaylistEditorViewModel : BaseViewModel
{
    private readonly IObservable<Func<IMapEntryBase, bool>> _filter;
    public readonly Player Player;
    private Playlist? _currentSelectedPlaylist;
    private ReadOnlyObservableCollection<IMapEntryBase>? _filteredSongEntries;
    private string _filterText;
    private bool _isDeletePlaylistPopupOpen;
    private bool _isNewPlaylistPopupOpen;
    private bool _isRenamePlaylistPopupOpen;
    private string _newPlaylistNameText;
    private SourceList<Playlist>? _playlists;

    public PlaylistEditorViewModel(Player player)
    {
        Player = player;

        _filter = this.WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(20))
            .Select(BuildFilter);

        Player.SortingModeBindable.BindValueChanged(d => UpdateSorting(d.NewValue), true);

        Activator = new ViewModelActivator();

        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            var playlistStorage = new PlaylistStorage();

            CurrentSelectedPlaylist = Playlists.Items.FirstOrDefault(x => x.Id == playlistStorage.Container.LastSelectedPlaylist) ?? Playlists.Items.ElementAt(0);

            if (_filteredSongEntries == default)
                Player.SongSource.Value.Connect().Sort(SortExpressionComparer<IMapEntryBase>.Ascending(x => Player.CustomSorter(x, Player.SortingModeBindable.Value)))
                    .Filter(_filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
                    .Bind(out _filteredSongEntries).Subscribe();

            this.RaisePropertyChanged(nameof(FilteredSongEntries));
        });

        SelectedPlaylistItems = new List<IMapEntryBase>();
        SelectedSongListItems = new List<IMapEntryBase>();
    }

    public string FilterText
    {
        get => _filterText;
        set => this.RaiseAndSetIfChanged(ref _filterText, value);
    }

    public bool IsDeletePlaylistPopupOpen
    {
        get => _isDeletePlaylistPopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isDeletePlaylistPopupOpen, value);
    }

    public bool IsRenamePlaylistPopupOpen
    {
        get => _isRenamePlaylistPopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isRenamePlaylistPopupOpen, value);
    }

    public bool IsNewPlaylistPopupOpen
    {
        get => _isNewPlaylistPopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isNewPlaylistPopupOpen, value);
    }

    public string NewPlaylistNameText
    {
        get => _newPlaylistNameText;
        set => this.RaiseAndSetIfChanged(ref _newPlaylistNameText, value);
    }

    public Playlist? CurrentSelectedPlaylist
    {
        get => _currentSelectedPlaylist;
        set
        {
            _currentSelectedPlaylist = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(Playlists));
            PlaylistManager.SetCurrentPlaylist(value);
        }
    }

    public SourceList<Playlist> Playlists
    {
        get => _playlists;
        set => this.RaiseAndSetIfChanged(ref _playlists, value);
    }

    public List<IMapEntryBase>? SelectedSongListItems { get; set; }

    public List<IMapEntryBase>? SelectedPlaylistItems { get; set; }

    public ReadOnlyObservableCollection<IMapEntryBase>? FilteredSongEntries => _filteredSongEntries;

    /// <summary>
    /// Updates the <see cref="FilteredSongEntries" /> according to the <paramref name="sortingMode" />
    /// </summary>
    /// <param name="sortingMode">the <see cref="SortingMode" /> of the song list</param>
    private void UpdateSorting(SortingMode sortingMode = SortingMode.Title)
    {
        Player.SongSource.Value.Connect().Sort(SortExpressionComparer<IMapEntryBase>.Ascending(x => Player.CustomSorter(x, sortingMode)))
            .Filter(_filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
            .Bind(out _filteredSongEntries).Subscribe();
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

        return song =>
        {
            return searchQs.All(x =>
                song.Title.Contains(x, StringComparison.OrdinalIgnoreCase) ||
                song.Artist.Contains(x, StringComparison.OrdinalIgnoreCase));
        };
    }
}