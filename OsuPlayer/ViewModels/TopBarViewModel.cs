using System.Reactive.Disposables;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class TopBarViewModel : BaseViewModel, IActivatableViewModel
{
    private string currentSongText = "currently playing nothing";

    public TopBarViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public string CurrentSongText
    {
        get => currentSongText;
        set => this.RaiseAndSetIfChanged(ref currentSongText, value);
    }

    public ViewModelActivator Activator { get; }
}