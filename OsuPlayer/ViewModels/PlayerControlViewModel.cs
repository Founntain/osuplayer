using System;
using System.Reactive.Disposables;
using OsuPlayer.Modules.Extensions;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PlayerControlViewModel : BaseViewModel, IActivatableViewModel
{
    private string currentSongLength = "00:00";

    private string currentSongTime = "00:00";

    private bool isPlaying;

    private bool isShuffle;

    private double playbackSpeed;

    private double songLength;

    private double songTime;

    public PlayerControlViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public double Volume
    {
        get => Core.Instance.Config.Volume;
        set
        {
            Core.Instance.Config.Volume = value;
            Core.Instance.Engine.SetVolume((float) value / 100);
            this.RaisePropertyChanged();
        }
    }

    public bool IsShuffle
    {
        get => isShuffle;
        set => this.RaiseAndSetIfChanged(ref isShuffle, value);
    }

    public double PlaybackSpeed
    {
        get => playbackSpeed;
        set => this.RaiseAndSetIfChanged(ref playbackSpeed, value);
    }

    public double SongTime
    {
        get => songTime;
        set
        {
            this.RaiseAndSetIfChanged(ref songTime, value);
            CurrentSongTime = TimeSpan.FromSeconds(value).FormatTime();
        }
    }

    public string CurrentSongTime
    {
        get => currentSongTime;
        set => this.RaiseAndSetIfChanged(ref currentSongTime, value);
    }

    public double SongLength
    {
        get => songLength;
        set
        {
            this.RaiseAndSetIfChanged(ref songLength, value);
            CurrentSongLength = TimeSpan.FromSeconds(value).FormatTime();
        }
    }

    public string CurrentSongLength
    {
        get => currentSongLength;
        set => this.RaiseAndSetIfChanged(ref currentSongLength, value);
    }

    public bool IsPlaying
    {
        get => isPlaying;
        set => this.RaiseAndSetIfChanged(ref isPlaying, value);
    }

    public ViewModelActivator Activator { get; }
}