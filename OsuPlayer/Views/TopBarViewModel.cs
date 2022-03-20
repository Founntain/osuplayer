using System.Reactive.Disposables;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class TopBarViewModel : BaseViewModel
{
    private string _currentSongText = "currently playing nothing";

    public TopBarViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }
}