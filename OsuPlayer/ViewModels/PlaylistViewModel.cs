using System.Reactive.Disposables;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PlaylistViewModel : BaseViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public PlaylistViewModel()
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