using System.Reactive.Disposables;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class TopBarViewModel : BaseViewModel, IActivatableViewModel
{
    private string _currentSongText = "currently playing nothing";

    public TopBarViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public string CurrentSongText
    {
        get => _currentSongText;
        set => this.RaiseAndSetIfChanged(ref _currentSongText, value);
    }

    public ViewModelActivator Activator { get; }
}