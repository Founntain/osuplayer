using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Modules.Services;
using ReactiveUI;
using SkiaSharp;

namespace OsuPlayer.Views;

public class HomeViewModel : BaseViewModel
{
    private readonly Bindable<bool> _songsLoading = new();

    public readonly IPlayer Player;
    private readonly ReadOnlyObservableCollection<IMapEntryBase>? _sortedSongEntries;
    private readonly BindableList<ObservableValue> _graphValues = new();
    private Bitmap? _profilePicture;
    private readonly IStatisticsProvider? _statisticsProvider;

    public ReadOnlyObservableCollection<IMapEntryBase>? SortedSongEntries => _sortedSongEntries;
    public ObservableCollection<ISeries> Series { get; set; }

    public Axis[] Axes { get; set; } =
    {
        new()
        {
            IsVisible = false,
            Labels = null
        }
    };

    public bool IsUserNotLoggedIn => CurrentUser == default || CurrentUser?.Id == Guid.Empty;
    public bool IsUserLoggedIn => CurrentUser != default && CurrentUser?.Id != Guid.Empty;

    public bool SongsLoading => new Config().Container.OsuPath != null && _songsLoading.Value;

    public User? CurrentUser => ProfileManager.User;

    public Bitmap? ProfilePicture
    {
        get => _profilePicture;
        set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
    }

    public HomeViewModel(IPlayer player, IStatisticsProvider? statisticsProvider, ISortProvider? sortProvider)
    {
        Player = player;
        _statisticsProvider = statisticsProvider;

        _songsLoading.BindTo(Player.SongsLoading);
        _songsLoading.BindValueChanged(d => this.RaisePropertyChanged(nameof(SongsLoading)));

        if (_statisticsProvider != null)
        {
            _graphValues.BindTo(_statisticsProvider.GraphValues);
            _graphValues.BindCollectionChanged((sender, args) =>
            {
                Series!.First().Values = _graphValues;

                this.RaisePropertyChanged(nameof(Series));
            });

            _statisticsProvider.UserDataChanged += (sender, args) => this.RaisePropertyChanged(nameof(CurrentUser));
        }

        sortProvider?.SortedSongs?.ObserveOn(AvaloniaScheduler.Instance).Bind(out _sortedSongEntries).Subscribe();

        this.RaisePropertyChanged(nameof(SortedSongEntries));

        Activator = new ViewModelActivator();

        this.WhenActivated(Block);
    }

    private IEnumerable<double> GetValues()
    {
        var rdm = new Random();

        for (var x = 0; x < 25; x++)
            yield return rdm.Next(25, 50);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        ProfilePicture = await LoadProfilePicture();

        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<ObservableValue>
            {
                Name = "XP gained",
                Values = _graphValues,
                Fill = new LinearGradientPaint(new[]
                {
                    new SKColor(128, 0, 128, 0),
                    new SKColor(128, 0, 128, 25),
                    new SKColor(128, 0, 128, 50),
                    new SKColor(128, 0, 128, 75),
                    new SKColor(128, 0, 128, 100),
                    new SKColor(128, 0, 128, 125),
                    new SKColor(128, 0, 128, 150),
                    new SKColor(128, 0, 128, 175),
                    new SKColor(128, 0, 128, 200),
                    new SKColor(128, 0, 128, 225),
                    SKColors.Purple
                }, new SKPoint(.5f, 1f), new SKPoint(.5f, 0f)),
                Stroke = new SolidColorPaint(SKColors.MediumPurple),
                GeometrySize = 0,
                GeometryFill = new SolidColorPaint(SKColors.MediumPurple),
                GeometryStroke = new SolidColorPaint(SKColors.Purple)
            }
        };
    }

    internal async Task<Bitmap?> LoadProfilePicture()
    {
        if (CurrentUser == default) return default;

        var profilePicture = await ApiAsync.GetProfilePictureAsync(CurrentUser.Name);

        if (profilePicture == default) return default;

        await using (var stream = new MemoryStream(Convert.FromBase64String(profilePicture)))
        {
            return await Task.Run(() => new Bitmap(stream));
        }
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