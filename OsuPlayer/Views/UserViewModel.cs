using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Material.Icons;
using Material.Icons.Avalonia;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Api.Data.API.RequestModels.Statistics;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Network.API.Service.Endpoints;
using ReactiveUI;
using SkiaSharp;
using Splat;

namespace OsuPlayer.Views;

public class UserViewModel : BaseViewModel
{
    public readonly IPlayer Player;
    private ObservableCollection<IControl>? _badges;
    private CancellationTokenSource? _bannerCancellationTokenSource;
    private Bitmap? _currentProfileBanner;
    private Bitmap? _currentProfilePicture;
    private CancellationTokenSource? _profilePictureCancellationTokenSource;
    private User? _selectedUser;
    private List<ObservableValue>? _songsPlayedGraphValues;
    private CancellationTokenSource? _topSongsCancellationTokenSource;
    private ObservableCollection<BeatmapTimesPlayedModel>? _topSongsOfCurrentUser;
    private ObservableCollection<User>? _users;
    private List<ObservableValue>? _xpGainedGraphValues;

    public ObservableCollection<ISeries>? Series { get; set; }

    public Axis[] YAxes { get; set; } =
    {
        new()
        {
            IsVisible = false,
            Labels = null
        }
    };

    public Axis[] XAxes { get; set; } =
    {
        new()
        {
            IsVisible = true,
            LabelsPaint = new SolidColorPaint(SKColors.White),
            LabelsRotation = 45
        }
    };

    public List<ObservableValue> XpGainedGraphValues
    {
        get => _xpGainedGraphValues ?? new List<ObservableValue>();
        private set
        {
            if (Series != default)
            {
                Series[1].Values = value;
                this.RaisePropertyChanged(nameof(Series));
            }

            this.RaisePropertyChanged(nameof(SelectedUser));
            this.RaiseAndSetIfChanged(ref _xpGainedGraphValues, value);
        }
    }

    public List<ObservableValue> SongsPlayedGraphValues
    {
        get => _songsPlayedGraphValues ?? new List<ObservableValue>();
        private set
        {
            if (Series != default)
            {
                Series[0].Values = value;
                this.RaisePropertyChanged(nameof(Series));
            }

            this.RaisePropertyChanged(nameof(SelectedUser));
            this.RaiseAndSetIfChanged(ref _songsPlayedGraphValues, value);
        }
    }

    public ObservableCollection<BeatmapTimesPlayedModel>? TopSongsOfCurrentUser
    {
        get => _topSongsOfCurrentUser;
        set => this.RaiseAndSetIfChanged(ref _topSongsOfCurrentUser, value);
    }

    public ObservableCollection<IControl> Badges
    {
        get => _badges ?? new ObservableCollection<IControl>();
        set => this.RaiseAndSetIfChanged(ref _badges, value);
    }

    public Bitmap? CurrentProfileBanner
    {
        get => _currentProfileBanner;
        set => this.RaiseAndSetIfChanged(ref _currentProfileBanner, value);
    }

    public Bitmap? CurrentProfilePicture
    {
        get => _currentProfilePicture;
        set => this.RaiseAndSetIfChanged(ref _currentProfilePicture, value);
    }

    public ObservableCollection<User> Users
    {
        get => _users ?? new ObservableCollection<User>();
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }

    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (Equals(_selectedUser, value)) return;

            this.RaiseAndSetIfChanged(ref _selectedUser, value);

            if (value == null) return;

            ReloadStats();
        }
    }

    public UserViewModel(IPlayer player)
    {
        Player = player;

        Activator = new ViewModelActivator();

        Badges = new ObservableCollection<IControl>();

        SongsPlayedGraphValues = new List<ObservableValue>();
        XpGainedGraphValues = new List<ObservableValue>();

        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        Users = (await Locator.Current.GetService<NorthFox>().GetAllUsers())
            ?.Select(x => new User(x))
            .ToObservableCollection() ?? new ObservableCollection<User>();

        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<ObservableValue>
            {
                Name = "Songs played",
                Values = SongsPlayedGraphValues,
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
                GeometrySize = 5,
                GeometryFill = new SolidColorPaint(SKColors.MediumPurple),
                GeometryStroke = new SolidColorPaint(SKColors.White)
            },
            new LineSeries<ObservableValue>
            {
                Name = "XP gained",
                Values = XpGainedGraphValues,
                Fill = new LinearGradientPaint(new[]
                {
                    new SKColor(128, 0, 0, 0),
                    new SKColor(128, 0, 0, 25),
                    new SKColor(128, 0, 0, 50),
                    new SKColor(128, 0, 0, 75),
                    new SKColor(128, 0, 0, 100),
                    new SKColor(128, 0, 0, 125),
                    new SKColor(128, 0, 0, 150),
                    new SKColor(128, 0, 0, 175),
                    new SKColor(128, 0, 0, 200),
                    new SKColor(128, 0, 0, 225),
                    SKColors.Purple
                }, new SKPoint(.5f, 1f), new SKPoint(.5f, 0f)),
                Stroke = new SolidColorPaint(SKColors.MediumVioletRed),
                GeometrySize = 5,
                GeometryFill = new SolidColorPaint(SKColors.MediumVioletRed),
                GeometryStroke = new SolidColorPaint(SKColors.White)
            }
        };

        if (SelectedUser?.Name != default) return;

        var user = ProfileManager.User;

        if (user?.Name == null)
        {
            if (Users == default) return;

            SelectedUser = Users.FirstOrDefault();

            return;
        }

        SelectedUser = user;
    }

    private void ReloadStats()
    {
        LoadTopSongs();
        LoadProfilePicture();
        LoadProfileBanner();
        LoadStats();
    }

    private async void LoadTopSongs()
    {
        if (SelectedUser == default || string.IsNullOrWhiteSpace(SelectedUser.Name))
        {
            TopSongsOfCurrentUser = default;
            return;
        }

        _topSongsCancellationTokenSource?.Cancel();
        _topSongsCancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = _topSongsCancellationTokenSource.Token;

        try
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var stats = await Locator.Current.GetService<NorthFox>().GetBeatmapsPlayedByUser(SelectedUser.UniqueId);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            TopSongsOfCurrentUser = stats?.Beatmaps?.ToObservableCollection();
        }
        catch (OperationCanceledException)
        {
            TopSongsOfCurrentUser = default;
            Debug.WriteLine("OPERATION CANCELED");
        }
    }

    private async void LoadProfilePicture()
    {
        if (SelectedUser == default || SelectedUser.UniqueId == Guid.Empty)
        {
            CurrentProfilePicture = default;
            return;
        }

        _profilePictureCancellationTokenSource?.Cancel();
        _profilePictureCancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = _profilePictureCancellationTokenSource.Token;

        try
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var profilePicture = await Locator.Current.GetService<NorthFox>().GetProfilePictureAsync(SelectedUser.UniqueId);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            CurrentProfilePicture = profilePicture;
        }
        catch (OperationCanceledException)
        {
            CurrentProfilePicture = default;
            Debug.WriteLine("OPERATION CANCELED");
        }
    }

    private async void LoadProfileBanner()
    {
        if (SelectedUser == default)
        {
            CurrentProfileBanner = default;
            return;
        }

        _bannerCancellationTokenSource?.Cancel();
        _bannerCancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = _bannerCancellationTokenSource.Token;

        try
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var banner = await Locator.Current.GetService<NorthFox>().GetProfileBannerAsync(SelectedUser.CustomBannerUrl);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            if (banner == default)
            {
                CurrentProfileBanner = default;
                return;
            }

            CurrentProfileBanner = banner;
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("OPERATION CANCELED");

            CurrentProfileBanner = default;
        }
    }

    private async void LoadStats()
    {
        if (SelectedUser == default || SelectedUser.UniqueId == Guid.Empty) return;

        var data = await Locator.Current.GetService<NorthFox>().GetActivityOfUser(SelectedUser.UniqueId);

        if (data == default) return;

        XAxes.First().Labels = new List<string>();

        var songsPlayedValues = new List<ObservableValue>();
        var xpGainedValues = new List<ObservableValue>();

        foreach (var item in data)
            try
            {
                XAxes.First().Labels?.Add(item.Date.ToString("MMMM yyyy"));

                songsPlayedValues.Add(new ObservableValue(item.SongsPlayed));
                xpGainedValues.Add(new ObservableValue(item.XpGained));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        SongsPlayedGraphValues = songsPlayedValues;
        XpGainedGraphValues = xpGainedValues;

        this.RaisePropertyChanged(nameof(XAxes));
    }

    public IEnumerable<IControl> LoadBadges(User? currentUser)
    {
        return new List<IControl>();

        if (currentUser == default) return default!;

        var badges = new List<MaterialIcon>();

        var size = 32;

        foreach (var badge in currentUser.Badges)
        {
            var materialIcon = new MaterialIcon
            {
                Kind = (MaterialIconKind) badge.Icon,
                Height = size,
                Width = size
            };

            badges.Add(materialIcon);
        }

        return badges;
    }
}