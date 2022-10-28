using System.Reactive.Disposables;
using OsuPlayer.Base.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class StatisticsViewModel : BaseViewModel
{
    public StatisticsViewModel()
    {
        Activator = new ViewModelActivator();

        this.WhenActivated(Block);
    }

    private void Block(CompositeDisposable disposable)
    {
        
    }
}