using System.Reactive.Disposables;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Network.API.Service.Endpoints;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public class PartyViewModel : BaseViewModel
{
    private ObservableCollection<PartyModel> _availableParties;

    public ObservableCollection<PartyModel> AvailableParties
    {
        get => _availableParties;
        set => this.RaiseAndSetIfChanged(ref _availableParties, value);
    }

    public PartyViewModel()
    {
        Activator = new ViewModelActivator();
        
        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);
        
        var player = Locator.Current.GetService<IPlayer>();
        var users = await Locator.Current.GetService<NorthFox>().GetAllUsers();

        var testUsers = users!.Take(10).ToArray();

        AvailableParties = new List<PartyModel>()
        {
            new()
            {
                HostId = testUsers.First().UniqueId,
                Beatmap = new ()
                {
                    Title = player.CurrentSong.Value.Title,
                    Artist = player.CurrentSong.Value.Artist,
                },
                IsPrivate = false,
                IsPaused = true,
                UsersInParty = testUsers.ToHashSet()
            },
            new()
            {
                HostId = testUsers.Take(5).Last().UniqueId,
                Beatmap = new ()
                {
                    Title = player.CurrentSong.Value.Title,
                    Artist = player.CurrentSong.Value.Artist,
                },
                IsPrivate = false,
                IsPaused = true,
                UsersInParty = testUsers.Take(5).ToHashSet()
            }
        }.ToObservableCollection();
    }

    private PartyModel _selectedParty;

    public PartyModel SelectedParty
    {
        get => _selectedParty;
        set => this.RaiseAndSetIfChanged(ref _selectedParty, value);
    }
}