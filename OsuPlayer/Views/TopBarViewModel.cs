using System.Reactive.Disposables;
using OsuPlayer.ViewModels;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public class TopBarViewModel : BaseViewModel
{
    public TopBarViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }
}