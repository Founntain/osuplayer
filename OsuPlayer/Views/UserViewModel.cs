using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Material.Icons;
using Material.Icons.Avalonia;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.API.Models.Beatmap;
using OsuPlayer.Extensions;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class UserViewModel : BaseViewModel
{
    public readonly Player Player;
    private ObservableCollection<IControl> _badges;
    private CancellationTokenSource? _bannerCancellationTokenSource;
    private Bitmap? _currentProfileBanner;
    private Bitmap? _currentProfilePicture;
    private CancellationTokenSource? _profilePictureCancellationTokenSource;
    private User? _selectedUser;
    private CancellationTokenSource? _topSongsCancellationTokenSource;
    private ObservableCollection<BeatmapUserValidityModel> _topSongsOfCurrentUser;

    private ObservableCollection<User> _users;

    public UserViewModel(Player player)
    {
        Player = player;

        Activator = new ViewModelActivator();

        Badges = new ObservableCollection<IControl>();

        this.WhenActivated(Block);
    }

    public ObservableCollection<BeatmapUserValidityModel> TopSongsOfCurrentUser
    {
        get => _topSongsOfCurrentUser;
        set => this.RaiseAndSetIfChanged(ref _topSongsOfCurrentUser, value);
    }

    public ObservableCollection<IControl> Badges
    {
        get => _badges;
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
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }

    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedUser, value);

            LoadTopSongs();
            LoadProfilePicture();
            LoadProfileBanner();
        }
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        Users = (await ApiAsync.GetRequestAsync<List<User>>("users", "getUsersWithData")).ToObservableCollection();

        var user = ProfileManager.User;

        if (user == default)
        {
            if (Users == default) return;

            SelectedUser = Users.FirstOrDefault();

            return;
        }

        SelectedUser = user;
    }

    private async void LoadTopSongs()
    {
        if (SelectedUser == default)
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

            var songs = await ApiAsync.GetBeatmapsPlayedByUser(SelectedUser.Name);

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

    private async void LoadProfilePicture()
    {
        if (SelectedUser == default)
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

            var profilePicture = await ApiAsync.GetProfilePictureAsync(SelectedUser.Name);

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

            var banner = await ApiAsync.GetProfileBannerAsync(SelectedUser.CustomWebBackground);

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

    public IEnumerable<IControl> LoadBadges(User currentUser)
    {
        if (currentUser == default) return default!;

        var badges = new List<MaterialIcon>();

        var size = 32;

        if (currentUser.Role == UserRole.Developer)
            badges.Add(new MaterialIcon
            {
                Kind = MaterialIconKind.Xml,
                Height = size,
                Width = size
            });

        if (currentUser.IsDonator)
            badges.Add(new MaterialIcon
            {
                Kind = MaterialIconKind.Heart,
                Height = size,
                Width = size
            });

        if (currentUser.Role == UserRole.Tester)
            badges.Add(new MaterialIcon
            {
                Kind = MaterialIconKind.TestTube,
                Height = size,
                Width = size
            });

        if (currentUser.JoinDate < new DateTime(2019, 1, 1))
            badges.Add(new MaterialIcon
            {
                Kind = MaterialIconKind.Creation,
                Height = size,
                Width = size
            });

        return badges;
    }
}