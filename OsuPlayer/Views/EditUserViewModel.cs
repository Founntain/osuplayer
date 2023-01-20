using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.Media.Imaging;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.Statistics;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Extensions;
using OsuPlayer.Network.API.Service.Endpoints;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public class EditUserViewModel : BaseViewModel
{
    private CancellationTokenSource? _bannerCancellationTokenSource;
    private string _confirmDeletionPassword = string.Empty;
    private Bitmap? _currentProfileBanner;
    private Bitmap? _currentProfilePicture;

    private UserModel? _currentUser;
    private bool _isDeleteProfilePopupOpen;
    private bool _isNewBannerSelected;
    private bool _isNewProfilePictureSelected;
    private string _newPassword = string.Empty;
    private string _newUsername = ProfileManager.User?.Name ?? string.Empty;
    private string _password = string.Empty;
    private CancellationTokenSource? _profilePictureCancellationTokenSource;
    private CancellationTokenSource? _topSongsCancellationTokenSource;
    private ObservableCollection<BeatmapTimesPlayedModel>? _topSongsOfCurrentUser;

    public string ConfirmDeletionPassword
    {
        get => _confirmDeletionPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmDeletionPassword, value);
    }

    public bool IsDeleteProfilePopupOpen
    {
        get => _isDeleteProfilePopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isDeleteProfilePopupOpen, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public bool IsNewBannerSelected
    {
        get => _isNewBannerSelected;
        set => this.RaiseAndSetIfChanged(ref _isNewBannerSelected, value);
    }

    public bool IsNewProfilePictureSelected
    {
        get => _isNewProfilePictureSelected;
        set => this.RaiseAndSetIfChanged(ref _isNewProfilePictureSelected, value);
    }

    public ObservableCollection<BeatmapTimesPlayedModel>? TopSongsOfCurrentUser
    {
        get => _topSongsOfCurrentUser;
        set => this.RaiseAndSetIfChanged(ref _topSongsOfCurrentUser, value);
    }

    public Bitmap? CurrentProfilePicture
    {
        get => _currentProfilePicture;
        set => this.RaiseAndSetIfChanged(ref _currentProfilePicture, value);
    }

    public Bitmap? CurrentProfileBanner
    {
        get => _currentProfileBanner;
        set => this.RaiseAndSetIfChanged(ref _currentProfileBanner, value);
    }

    public string? CurrentProfileBannerUrl
    {
        get => _currentUser?.CustomBannerUrl;
        set => _currentUser!.CustomBannerUrl = value;
    }

    public UserModel? CurrentUser
    {
        get => _currentUser;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentUser, value);

            // LoadTopSongs();
            LoadProfilePicture();
            LoadProfileBanner();
        }
    }

    public string NewPassword
    {
        get => _newPassword;
        set => this.RaiseAndSetIfChanged(ref _newPassword, value);
    }

    public string NewUsername
    {
        get => _newUsername;
        set => this.RaiseAndSetIfChanged(ref _newUsername, value);
    }

    public EditUserViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            CurrentUser = ProfileManager.User;
        });
    }

    private async void LoadTopSongs()
    {
        if (CurrentUser == default || string.IsNullOrWhiteSpace(CurrentUser.Name))
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

            var stats = await Locator.Current.GetService<NorthFox>().GetBeatmapsPlayedByUser(CurrentUser.UniqueId);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            TopSongsOfCurrentUser = stats.Beatmaps.ToObservableCollection();
        }
        catch (OperationCanceledException)
        {
            TopSongsOfCurrentUser = default;
            Debug.WriteLine("OPERATION CANCELED");
        }
    }

    public async void LoadProfilePicture()
    {
        if (CurrentUser == default)
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

            var profilePicture = await Locator.Current.GetService<NorthFox>().GetProfilePictureAsync(CurrentUser.UniqueId);
            
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

    public async void LoadProfileBanner()
    {
        if (CurrentUser == default)
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

            var banner = await Locator.Current.GetService<NorthFox>().GetProfileBannerAsync(CurrentUser.CustomBannerUrl);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            if (banner == default)
            {
                CurrentProfileBanner = default;
                return;
            }

            CurrentProfileBanner = banner;
            CurrentProfileBannerUrl = CurrentUser.CustomBannerUrl;
            this.RaisePropertyChanged(nameof(CurrentProfileBannerUrl));
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("OPERATION CANCELED");

            CurrentProfileBanner = default;
        }
    }
}