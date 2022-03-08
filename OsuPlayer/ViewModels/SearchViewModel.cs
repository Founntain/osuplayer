using System.Reactive.Disposables;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class SearchViewModel : BaseViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }
    public string FilterText { get; set; }

    public SearchViewModel()
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