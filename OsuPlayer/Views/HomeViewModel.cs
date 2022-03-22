using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;
using OsuPlayer.ViewModels;
using ReactiveUI;
using SkiaSharp;

namespace OsuPlayer.Views;

public class HomeViewModel : BaseViewModel
{
    private Bitmap? _profilePicture;

    private bool _songsLoading;

    public HomeViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(Block);
    }

    public ISeries[] Series { get; set; } =
        {
            new LineSeries<double>
            {
                Name = "XP gained",
                Values = GetValues(),
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

    public Axis[] Axes { get; set; } =
        {
            new()
            {
                IsVisible = false,
                Labels = null
            }
        };

    public List<MapEntry> SongEntries => Core.Instance.Player.SongSource!;

    public bool IsUserNotLoggedIn => CurrentUser == default;
    public bool IsUserLoggedIn => CurrentUser != default;

    public bool SongsLoading
    {
        get => new Config().Read().OsuPath != null && _songsLoading;
        set => this.RaiseAndSetIfChanged(ref _songsLoading, value);
    }

    public User? CurrentUser => ProfileManager.User;

    public Bitmap? ProfilePicture
    {
        get => _profilePicture;
        set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
    }

    private static IEnumerable<double> GetValues()
    {
        var rdm = new Random();

        for (var x = 0; x < 25; x++)
            yield return rdm.Next(25, 50);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        ProfilePicture = await LoadProfilePicture();
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