using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class BaseViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; protected init; }
}