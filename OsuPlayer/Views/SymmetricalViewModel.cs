using System.Reactive.Disposables;
using Nein.Base;
using ReactiveUI;

namespace OsuPlayer.Views;

public class SymmetricalViewModel : BaseViewModel
{
    public SymmetricalViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }
}