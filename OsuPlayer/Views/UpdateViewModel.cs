using System.Reactive.Disposables;
using OsuPlayer.Network;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class UpdateViewModel : BaseViewModel
{
    public UpdateResponse Update
    {
        get => _update;
        set => this.RaiseAndSetIfChanged(ref _update, value);
    }
    
    private UpdateResponse _update;
    
    public UpdateViewModel()
    {
        Activator = new ViewModelActivator();

        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }
}