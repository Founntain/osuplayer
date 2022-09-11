using ReactiveUI;

namespace OsuPlayer.Base.ViewModels;

public class BaseWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new();
}