using System.Reactive.Disposables;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class UserViewModel : BaseViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public UserViewModel()
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