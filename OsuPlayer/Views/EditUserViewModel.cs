using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.Media.Imaging;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Data.API.Models.Beatmap;
using OsuPlayer.Extensions;
using ReactiveUI;

namespace OsuPlayer.Views;

public class EditUserViewModel : BaseViewModel
{
    private CancellationTokenSource? _bannerCancellationTokenSource;
    private string _confirmDeletionPassword;
    private Bitmap? _currentProfileBanner;
    private Bitmap? _currentProfilePicture;

    private User? _currentUser;
    private bool _isDeleteProfilePopupOpen;
    private bool _isNewBannerSelected;
    private bool _isNewProfilePictureSelected;
    private string _newPassword;
    private string _password;
    private CancellationTokenSource? _profilePictureCancellationTokenSource;
    private CancellationTokenSource? _topSongsCancellationTokenSource;
    private ObservableCollection<BeatmapUserValidityModel>? _topSongsOfCurrentUser;

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

    public ObservableCollection<BeatmapUserValidityModel>? TopSongsOfCurrentUser
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

    public string CurrentProfileBannerUrl
    {
        get => _currentUser?.CustomWebBackground ?? string.Empty;
        set => _currentUser!.CustomWebBackground = value;
    }

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentUser, value);

            LoadTopSongs();
            LoadProfilePicture();
            LoadProfileBanner();
        }
    }

    public string NewPassword
    {
        get => _newPassword;
        set => this.RaiseAndSetIfChanged(ref _newPassword, value);
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
        if (CurrentUser == default)
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

            var songs = await ApiAsync.GetBeatmapsPlayedByUser(CurrentUser.Name);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            TopSongsOfCurrentUser = songs.ToObservableCollection();
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

            var profilePicture = await ApiAsync.GetProfilePictureAsync(CurrentUser.Name);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(profilePicture))
            {
                CurrentProfilePicture = default;
                return;
            }

            await using (var stream = new MemoryStream(Convert.FromBase64String(profilePicture)))
            {
                try
                {
                    var bitmap = new Bitmap(stream);

                    CurrentProfilePicture = bitmap;

                    Debug.WriteLine(bitmap.ToString());
                }
                catch (Exception)
                {
                    CurrentProfilePicture = default;
                    Debug.WriteLine("Could not convert ProfilePicture MemoryStream to Bitmap");
                }
            }
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

            var banner = await ApiAsync.GetProfileBannerAsync(CurrentUser.CustomWebBackground);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            if (banner == default)
            {
                CurrentProfileBanner = default;
                return;
            }

            CurrentProfileBanner = banner;
            CurrentProfileBannerUrl = CurrentUser.CustomWebBackground;
            this.RaisePropertyChanged(nameof(CurrentProfileBannerUrl));
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("OPERATION CANCELED");

            CurrentProfileBanner = default;
        }
    }
}