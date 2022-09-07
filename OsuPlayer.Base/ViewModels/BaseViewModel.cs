using ReactiveUI;

namespace OsuPlayer.Base.ViewModels;

public class BaseViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; protected init; }
}