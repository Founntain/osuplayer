using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using DynamicData;
using Nein.Base;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;

namespace OsuPlayer.Views;

public class BlacklistEditorViewModel : BaseViewModel
{
    private readonly ReadOnlyObservableCollection<IMapEntryBase>? _filteredSongEntries;

    public readonly IPlayer Player;
    private BlacklistContainer? _blacklist;
    private string _filterText = string.Empty;

    public List<IMapEntryBase>? SelectedSongListItems { get; set; }
    public List<IMapEntryBase>? SelectedBlacklistItems { get; set; }

    public string FilterText
    {
        get => _filterText;
        set => this.RaiseAndSetIfChanged(ref _filterText, value);
    }

    public BlacklistContainer? Blacklist
    {
        get => _blacklist;
        set => this.RaiseAndSetIfChanged(ref _blacklist, value);
    }

    public ReadOnlyObservableCollection<IMapEntryBase>? FilteredSongEntries => _filteredSongEntries;

    public BlacklistEditorViewModel(IPlayer player)
    {
        Activator = new ViewModelActivator();

        Player = player;

        Player.BlacklistChanged += (_, _) => Dispatcher.UIThread.Post(() => Blacklist = new Blacklist().Container);

        //_sortProvider.SortingModeBindable.BindValueChanged(d => UpdateSorting(d.NewValue), true);
        var filter = this.WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(20))
            .Select(BuildFilter);

        player.SongSourceProvider.Songs?.Filter(filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
            .Bind(out _filteredSongEntries).Subscribe();

        this.RaisePropertyChanged(nameof(FilteredSongEntries));

        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            Blacklist = new Blacklist().Container;
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