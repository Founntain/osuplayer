using System.Reactive.Disposables;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PartyViewModel : BaseViewModel
{
    public PartyViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }
}