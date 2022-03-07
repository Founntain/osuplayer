using System.Reactive.Disposables;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PartyViewModel : BaseViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public PartyViewModel()
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