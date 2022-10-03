using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Modules;
using ReactiveUI;
using SkiaSharp;
using Splat;

namespace OsuPlayer.Views;

public class HomeViewModel : BaseViewModel
{
    private readonly Bindable<bool> _songsLoading = new();

    public readonly IPlayer Player;
    private readonly BindableList<ObservableValue> _graphValues = new();
    private Bitmap? _profilePicture;
    private readonly IStatisticsProvider? _statisticsProvider;

    public ObservableCollection<ISeries> Series { get; set; }

    public Axis[] Axes { get; set; } =
    {
        new()
        {
            IsVisible = false,
            Labels = null
        }
    };

    public List<IMapEntryBase> SongEntries => Player.SongSourceList!;

    public bool IsUserNotLoggedIn => CurrentUser == default || CurrentUser?.Id == Guid.Empty;
    public bool IsUserLoggedIn => CurrentUser != default && CurrentUser?.Id != Guid.Empty;

    public bool SongsLoading => new Config().Container.OsuPath != null && _songsLoading.Value;

    public User? CurrentUser => ProfileManager.User;

    public Bitmap? ProfilePicture
    {
        get => _profilePicture;
        set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
    }

    public HomeViewModel(IPlayer player)
    {
        Player = player;
        _statisticsProvider = Locator.Current.GetService<IStatisticsProvider>();

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

        Player.SongSource.BindValueChanged(d => this.RaisePropertyChanged(nameof(SongEntries)), true);

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
}