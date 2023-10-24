using System.Reactive.Disposables;
using System.Threading.Tasks;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.Enums;
using OsuPlayer.Api.Data.API.RequestModels.Beatmap;
using OsuPlayer.Api.Data.API.ResponseModels;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Network.API.NorthFox;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public class BeatmapsViewModel : BaseViewModel
{
    private ObservableCollection<BeatmapModel> _beatmaps = new();

    private int _currentPage;
    private int _totalPages;
    private string _searchArtist;
    private string _searchTitle;
    private string _searchBeatmapSetId;
    private string _searchBeatmapId;
    private FilterCondition _searchArtistFilterCondition;
    private FilterCondition _searchTitleFilterCondition;
    private FilterCondition _searchBeatmapSetIdFilterCondition;
    private FilterCondition _searchBeatmapIdFilterCondition;
    private bool _searchingBeatmaps;
    private bool _isFilterMenuOpen;

    public bool SearchingBeatmaps
    {
        get => _searchingBeatmaps;
        set => this.RaiseAndSetIfChanged(ref _searchingBeatmaps, value);
    }

    public int TotalPages
    {
        get => _totalPages;
        set => this.RaiseAndSetIfChanged(ref _totalPages, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    public bool IsFilterMenuOpen
    {
        get => _isFilterMenuOpen;
        set => this.RaiseAndSetIfChanged(ref _isFilterMenuOpen, value);
    }

    public string SearchBeatmapSetId
    {
        get => _searchBeatmapSetId;
        set => this.RaiseAndSetIfChanged(ref _searchBeatmapSetId, value);
    }

    public string SearchBeatmapId
    {
        get => _searchBeatmapId;
        set => this.RaiseAndSetIfChanged(ref _searchBeatmapId, value);
    }

    public string SearchTitle
    {
        get => _searchTitle;
        set => this.RaiseAndSetIfChanged(ref _searchTitle, value);
    }

    public string SearchArtist
    {
        get => _searchArtist;
        set => this.RaiseAndSetIfChanged(ref _searchArtist, value);
    }

    public FilterCondition SearchBeatmapSetIdFilterCondition
    {
        get => _searchBeatmapSetIdFilterCondition;
        set => this.RaiseAndSetIfChanged(ref _searchBeatmapSetIdFilterCondition, value);
    }

    public FilterCondition SearchBeatmapIdFilterCondition
    {
        get => _searchBeatmapIdFilterCondition;
        set => this.RaiseAndSetIfChanged(ref _searchBeatmapIdFilterCondition, value);
    }

    public FilterCondition SearchArtistFilterCondition
    {
        get => _searchArtistFilterCondition;
        set => this.RaiseAndSetIfChanged(ref _searchArtistFilterCondition, value);
    }

    public FilterCondition SearchTitleFilterCondition
    {
        get => _searchTitleFilterCondition;
        set => this.RaiseAndSetIfChanged(ref _searchTitleFilterCondition, value);
    }

    public IEnumerable<FilterCondition> StringFilterConditions => Enum.GetValues<FilterCondition>().Where(x => (int) x <= 2);

    public IEnumerable<FilterCondition> IntFilterConditions => Enum.GetValues<FilterCondition>().Where(x => (int) x > 2);

    public ObservableCollection<BeatmapModel> Beatmaps
    {
        get => _beatmaps;
        set => this.RaiseAndSetIfChanged(ref _beatmaps, value);
    }

    public IPlayer Player { get; }

    public BeatmapsViewModel(IPlayer player)
    {
        Player = player;

        Activator = new ViewModelActivator();

        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        SearchBeatmapIdFilterCondition = FilterCondition.GreaterOrEqualThan;
        SearchBeatmapSetIdFilterCondition = FilterCondition.GreaterOrEqualThan;

        // If we have beatmaps already loaded and reload the view we don't want to load them again
        if (Beatmaps?.Count > 0) return;

        await SearchBeatmaps();
    }

    public async Task<BeatmapSearchResponse> SearchBeatmaps(int newPage = 1, int pageSize = 64)
    {
        var api = Locator.Current.GetService<IOsuPlayerApiService>() as NorthFox;

        if (api == default) return new ();

        SearchingBeatmaps = true;

        var beatmaps = await api.Beatmap.GetBeatmapsPaged(new SearchBeatmapModel
        {
            Page = newPage,
            PageSize = pageSize,
            Artist = SearchArtist,
            ArtistFilterCondition = SearchArtistFilterCondition,
            Title = SearchTitle,
            TitleFilterCondition = SearchTitleFilterCondition,
            BeatmapSetId = SearchBeatmapSetId,
            BeatmapSetIdFilterCondition = SearchBeatmapSetIdFilterCondition,
            BeatmapId = SearchBeatmapId,
            BeatmapIdFilterCondition = SearchBeatmapIdFilterCondition
        });

        SearchingBeatmaps = false;

        if (beatmaps == null || beatmaps.Beatmaps.Count == 0) return beatmaps;

        Beatmaps = beatmaps.Beatmaps.ToObservableCollection();
        CurrentPage = beatmaps.CurrentPage;
        TotalPages = beatmaps.TotalPages;

        return beatmaps;
    }
}