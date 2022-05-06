using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class BaseWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new();
}