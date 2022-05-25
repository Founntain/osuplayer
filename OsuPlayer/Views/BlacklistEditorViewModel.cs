using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class BlacklistEditorViewModel : BaseViewModel
{
    private readonly IObservable<Func<IMapEntryBase, bool>> _filter;
    private BlacklistContainer _blacklist;
    private ReadOnlyObservableCollection<IMapEntryBase>? _filteredSongEntries;
    private string _filterText;

    public Player Player;

    public BlacklistEditorViewModel(Player player)
    {
        Activator = new ViewModelActivator();

        Player = player;

        Player.BlacklistChanged += (sender, args) => Blacklist = new Blacklist().Container;

        Player.SortingModeBindable.BindValueChanged(d => UpdateSorting(d.NewValue), true);

        _filter = this.WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(20))
            .Select(BuildFilter);

        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            Blacklist = new Blacklist().Container;

            if (_filteredSongEntries == default)
                Player.SongSource.Value.Connect().Sort(SortExpressionComparer<IMapEntryBase>.Ascending(x => Player.CustomSorter(x, Player.SortingModeBindable.Value)))
                    .Filter(_filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
                    .Bind(out _filteredSongEntries).Subscribe();

            this.RaisePropertyChanged(nameof(FilteredSongEntries));
        });
    }

    public List<IMapEntryBase>? SelectedSongListItems { get; set; }
    public List<IMapEntryBase>? SelectedBlacklistItems { get; set; }

    public string FilterText
    {
        get => _filterText;
        set => this.RaiseAndSetIfChanged(ref _filterText, value);
    }

    public BlacklistContainer Blacklist
    {
        get => _blacklist;
        set => this.RaiseAndSetIfChanged(ref _blacklist, value);
    }

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