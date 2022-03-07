using System;
using System.Reactive.Disposables;
using OsuPlayer.Extensions;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PlayerControlViewModel : BaseViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    private double volume;
    public double Volume
    {
        get => volume;
        set => this.RaiseAndSetIfChanged(ref volume, value);
    }

    private double playbackSpeed;
    public double PlaybackSpeed
    {
        get => playbackSpeed;
        set => this.RaiseAndSetIfChanged(ref playbackSpeed, value);
    }
    
    private double songTime;
    public double SongTime
    {
        get => songTime;
        set
        {
            this.RaiseAndSetIfChanged(ref songTime, value);
            CurrentSongTime = TimeSpan.FromSeconds(value).FormatTime();
        }
    }

    private string currentSongTime = "00:00";
    public string CurrentSongTime
    {
        get => currentSongTime;
        set => this.RaiseAndSetIfChanged(ref currentSongTime, value);
    }

    private double songLength;
    public double SongLength
    {
        get => songLength;
        set
        {
            this.RaiseAndSetIfChanged(ref songLength, value);
            CurrentSongLength = TimeSpan.FromSeconds(value).FormatTime();
        }
    }

    private string currentSongLength = "00:00";
    public string CurrentSongLength
    {
        get => currentSongLength;
        set => this.RaiseAndSetIfChanged(ref currentSongLength, value);
    }
    
    public PlayerControlViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable.Create(() =>
            {

            }).DisposeWith(disposables);
        });
    }
}