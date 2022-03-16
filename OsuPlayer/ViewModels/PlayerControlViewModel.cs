using System;
using System.Reactive.Disposables;
using OsuPlayer.Extensions;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PlayerControlViewModel : BaseViewModel, IActivatableViewModel
{
    private string _currentSongLength = "00:00";

    private string _currentSongTime = "00:00";

    private bool _isPlaying;

    private bool _isShuffle;

    private double _playbackSpeed;

    private double _songLength;

    private double _songTime;

    public PlayerControlViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public double Volume
    {
        get => Core.Instance.Player.Volume;
        set
        {
            Core.Instance.Player.Volume = value;
        }
    }

    public bool IsShuffle
    {
        get => _isShuffle;
        set => this.RaiseAndSetIfChanged(ref _isShuffle, value);
    }

    public double PlaybackSpeed
    {
        get => _playbackSpeed;
        set => this.RaiseAndSetIfChanged(ref _playbackSpeed, value);
    }

    public double SongTime
    {
        get => _songTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _songTime, value);
            CurrentSongTime = TimeSpan.FromSeconds(value).FormatTime();
        }
    }

    public string CurrentSongTime
    {
        get => _currentSongTime;
        set => this.RaiseAndSetIfChanged(ref _currentSongTime, value);
    }

    public double SongLength
    {
        get => _songLength;
        set
        {
            this.RaiseAndSetIfChanged(ref _songLength, value);
            CurrentSongLength = TimeSpan.FromSeconds(value).FormatTime();
        }
    }

    public string CurrentSongLength
    {
        get => _currentSongLength;
        set => this.RaiseAndSetIfChanged(ref _currentSongLength, value);
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set => this.RaiseAndSetIfChanged(ref _isPlaying, value);
    }

    public ViewModelActivator Activator { get; }
}