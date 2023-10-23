using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Nein.Base;
using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Network.API.NorthFox;
using OsuPlayer.Services;
using ReactiveUI;
using SkiaSharp;
using Splat;

namespace OsuPlayer.Views.HomeSubViews;

public class HomeUserPanelViewModel : BaseViewModel
{
    private readonly IProfileManagerService _profileManager;
    private readonly BindableList<ObservableValue> _graphValues = new();

    public bool IsUserNotLoggedIn => CurrentUser == default || CurrentUser?.UniqueId == Guid.Empty;
    public bool IsUserLoggedIn => CurrentUser != default && CurrentUser?.UniqueId != Guid.Empty;

    public IUser? CurrentUser => _profileManager.User;

    public Axis[] Axes { get; set; } =
    {
        new()
        {
            IsVisible = false,
            Labels = null
        }
    };

    private Bitmap? _profilePicture;

    public Bitmap? ProfilePicture
    {
        get => _profilePicture;
        set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
    }

    public ObservableCollection<ISeries> Series { get; set; }

    public HomeUserPanelViewModel(IStatisticsProvider? statisticsProvider, IProfileManagerService profileManager)
    {
        _profileManager = profileManager;

        var statsProvider = statisticsProvider;

        if (statsProvider != null)
        {
            _graphValues.BindTo(statsProvider.GraphValues);
            _graphValues.BindCollectionChanged((_, _) =>
            {
                Series!.First().Values = _graphValues;

                this.RaisePropertyChanged(nameof(Series));
            });

            statsProvider.UserDataChanged += (_, _) => this.RaisePropertyChanged(nameof(CurrentUser));
        }

        Activator = new ViewModelActivator();

        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable obj)
    {
        ProfilePicture = await LoadProfilePictureAsync();

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

        await LoadUserProfileAsync();
    }

    public async Task LoadUserProfileAsync()
    {
        if (!File.Exists("data/session.op"))
            return;

        var sessionToken = await File.ReadAllTextAsync("data/session.op");

        await _profileManager.Login(sessionToken);

        this.RaisePropertyChanged(nameof(IsUserLoggedIn));
        this.RaisePropertyChanged(nameof(IsUserNotLoggedIn));
        this.RaisePropertyChanged(nameof(CurrentUser));

        ProfilePicture = await LoadProfilePictureAsync();
    }

    private async Task<Bitmap?> LoadProfilePictureAsync()
    {
        if (CurrentUser == default || CurrentUser.UniqueId == Guid.Empty) return default;

        return await Locator.Current.GetService<IOsuPlayerApiService>().User.GetProfilePictureAsync(CurrentUser.UniqueId);
    }
}