using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Threading;
using DynamicData;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.DbReader.Interfaces;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlaylistEditorViewModel : BaseViewModel
{
    private readonly ReadOnlyObservableCollection<IMapEntryBase>? _filteredSongEntries;
    public readonly IPlayer Player;
    private Playlist? _currentSelectedPlaylist;
    private string _filterText = string.Empty;
    private bool _isDeletePlaylistPopupOpen;
    private bool _isNewPlaylistPopupOpen;
    private bool _isRenamePlaylistPopupOpen;
    private string _newPlaylistNameText = string.Empty;
    private SourceList<Playlist>? _playlists;

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
        }
    }

    public SourceList<Playlist>? Playlists
    {
        get => _playlists;
        set => this.RaiseAndSetIfChanged(ref _playlists, value);
    }

    public List<IMapEntryBase>? SelectedSongListItems { get; set; } = new();

    public List<IMapEntryBase>? SelectedPlaylistItems { get; set; } = new();

    public ReadOnlyObservableCollection<IMapEntryBase>? FilteredSongEntries => _filteredSongEntries;

    public PlaylistEditorViewModel(IPlayer player)
    {
        Player = player;

        var filter = this.WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(20))
            .Select(BuildFilter);

        //Player.SortingModeBindable.BindValueChanged(d => UpdateSorting(d.NewValue), true);
        Playlists = PlaylistManager.GetAllPlaylists().ToSourceList();

        Player.PlaylistChanged += (_, _) => { Playlists = PlaylistManager.GetAllPlaylists().ToSourceList(); };

        player.SongSourceProvider.Songs?.Filter(filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
            .Bind(out _filteredSongEntries).Subscribe();

        this.RaisePropertyChanged(nameof(FilteredSongEntries));

        Activator = new ViewModelActivator();

        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            var config = new Config();

            CurrentSelectedPlaylist = Playlists?.Items.FirstOrDefault(x => x.Id == config.Container.SelectedPlaylist) ?? Playlists?.Items.ElementAt(0);
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

        return song =>
        {
            return searchQs.All(x =>
                song.Title.Contains(x, StringComparison.OrdinalIgnoreCase) ||
                song.Artist.Contains(x, StringComparison.OrdinalIgnoreCase));
        };
    }
}