using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Material.Icons;
using Material.Icons.Avalonia;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.API.Models.Beatmap;
using OsuPlayer.Extensions;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class UserViewModel : BaseViewModel
{
    private CancellationTokenSource? ProfilePictureCancellationTokenSource;
    private CancellationTokenSource? BannerCancellationTokenSource;
    private CancellationTokenSource? TopSongsCancellationTokenSource;

    private ObservableCollection<User> _users;
    private User? _selectedUser;
    private Bitmap? _currentProfilePicture;
    private Bitmap? _currentProfileBanner;
    private ObservableCollection<IControl> _badges;
    private ObservableCollection<BeatmapUserValidityModel> _topSongsOfCurrentUser;

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
            LoadBadges();
        }
    }

    public UserViewModel()
    {
        Activator = new ViewModelActivator();

        Badges = new();

        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        Users = (await ApiAsync.GetRequestAsync<List<User>>("users", "getUsersWithData")).ToObservableCollection();
    }

    private async void LoadTopSongs()
    {
        if (SelectedUser == default)
        {
            TopSongsOfCurrentUser = default;
            return;
        }

        TopSongsCancellationTokenSource?.Cancel();
        TopSongsCancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = TopSongsCancellationTokenSource.Token;

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

        ProfilePictureCancellationTokenSource?.Cancel();
        ProfilePictureCancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = ProfilePictureCancellationTokenSource.Token;

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

        BannerCancellationTokenSource?.Cancel();
        BannerCancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = BannerCancellationTokenSource.Token;

        try
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var banner = await ApiAsync.GetProfileBannerAsync(SelectedUser.CustomWebBackground);

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

    private void LoadBadges()
    {
        if (SelectedUser == default) return;

        var badges = new List<IControl>();

        var size = 32;

        if (SelectedUser.Role == UserRole.Developer)
            badges.Add(new MaterialIcon
            {
                Kind = MaterialIconKind.Xml,
                Height = size,
                Width = size
            });

        if (SelectedUser.IsDonator)
            badges.Add(new MaterialIcon
            {
                Kind = MaterialIconKind.Heart,
                Height = size,
                Width = size
            });

        if (SelectedUser.Role == UserRole.Tester)
            badges.Add(new MaterialIcon
            {
                Kind = MaterialIconKind.TestTube,
                Height = size,
                Width = size
            });

        if (SelectedUser.JoinDate < new DateTime(2019, 1, 1))
            badges.Add(new MaterialIcon
            {
                Kind = MaterialIconKind.Creation,
                Height = size,
                Width = size
            });

        Badges = badges.ToObservableCollection();
    }
}