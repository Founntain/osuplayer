using System.Reactive.Disposables;
using ABI.Windows.Devices.Sensors.Custom;
using Nein.Base;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Modules.Party;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PartyViewModel : BaseViewModel
{
    private PartyManager _partyManager;
    
    private Bindable<PartyModel?> _currentParty = new();

    public Bindable<PartyModel?> CurrentParty
    {
        get => _currentParty;
        set => this.RaiseAndSetIfChanged(ref _currentParty, value);
    }
    
    public PartyViewModel(PartyManager partyManager)
    {
        _partyManager = partyManager;
        
        _currentParty.BindTo(_partyManager.CurrentParty);
        _currentParty.BindValueChanged(_ => this.RaisePropertyChanged(nameof(CurrentParty)));
        
        Activator = new ViewModelActivator();
        
        this.WhenActivated(Block);
    }

    private void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);
    }
}