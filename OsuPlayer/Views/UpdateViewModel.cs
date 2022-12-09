using System.Reactive.Disposables;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Network;
using ReactiveUI;

namespace OsuPlayer.Views;

public class UpdateViewModel : BaseViewModel
{
    private UpdateResponse? _update;

    public UpdateResponse? Update
    {
        get => _update;
        set => this.RaiseAndSetIfChanged(ref _update, value);
    }

    public UpdateViewModel()
    {
        Activator = new ViewModelActivator();

        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }
}