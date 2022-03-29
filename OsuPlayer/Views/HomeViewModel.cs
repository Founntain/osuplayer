using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Modules.Audio;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;
using OsuPlayer.ViewModels;
using ReactiveUI;
using SkiaSharp;

namespace OsuPlayer.Views;

public class HomeViewModel : BaseViewModel
{
    private List<ObservableValue> _graphValues;
    private Bitmap? _profilePicture;

    private Bindable<bool> _songsLoading = new();

    public Player Player;

    public HomeViewModel(Player player)
    {
        Player = player;

        _songsLoading.BindTo(Player.SongsLoading);
        _songsLoading.BindValueChanged(d => this.RaisePropertyChanged(nameof(SongsLoading)));

        Player.GraphValues.BindValueChanged(d => GraphValues = d.NewValue, true);
        Player.SongSource.BindValueChanged(d => this.RaisePropertyChanged(nameof(SongEntries)), true);

        Player.UserChanged += (sender, args) => this.RaisePropertyChanged(nameof(CurrentUser));

        GraphValues = new List<ObservableValue>();

        Activator = new ViewModelActivator();
        this.WhenActivated(Block);
    }

    public List<ObservableValue> GraphValues
    {
        get => _graphValues;
        private set
        {
            if (Series != default)
            {
                Series.First().Values = value;
                this.RaisePropertyChanged(nameof(Series));
            }

            this.RaisePropertyChanged(nameof(CurrentUser));
            this.RaiseAndSetIfChanged(ref _graphValues, value);
        }
    }

    public ObservableCollection<ISeries> Series { get; set; }

    public Axis[] Axes { get; set; } =
    {
        new()
        {
            IsVisible = false,
            Labels = null
        }
    };

    public List<MinimalMapEntry> SongEntries => Player.SongSourceList!;

    public bool IsUserNotLoggedIn => CurrentUser == default;
    public bool IsUserLoggedIn => CurrentUser != default;

    public bool SongsLoading => new Config().Read().OsuPath != null && _songsLoading.Value;

    public User? CurrentUser => ProfileManager.User;

    public Bitmap? ProfilePicture
    {
        get => _profilePicture;
        set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
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
                Values = GraphValues,
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