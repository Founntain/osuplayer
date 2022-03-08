using System.Reactive.Disposables;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class SearchViewModel : BaseViewModel, IActivatableViewModel
{
    public SearchViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public string FilterText { get; set; }
    public ViewModelActivator Activator { get; }
}