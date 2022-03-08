using System.Reactive.Disposables;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class TopBarViewModel : BaseViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    private string currentSongText = "currently playing nothing";
    public string CurrentSongText
    {
        get => currentSongText;
        set => this.RaiseAndSetIfChanged(ref currentSongText, value);
    }
    
    public TopBarViewModel()
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