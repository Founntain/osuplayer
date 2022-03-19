using System.Reactive.Disposables;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PartyViewModel : BaseViewModel, IActivatableViewModel
{
    public PartyViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public ViewModelActivator Activator { get; }
}