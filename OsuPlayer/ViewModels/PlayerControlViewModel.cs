using System.Reactive.Disposables;
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

    private string currentSongTime = "00:00";
    public string CurrentSongTime
    {
        get => currentSongTime;
        set => this.RaiseAndSetIfChanged(ref currentSongTime, value);
    }
    
    private string currentSongTimeLeft = "00:00";
    public string CurrentSongTimeLeft
    {
        get => currentSongTimeLeft;
        set => this.RaiseAndSetIfChanged(ref currentSongTimeLeft, value);
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