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

    private int _users = 1234;
    private int _translators = 1234;
    private int _songsPlayed = 1234;
    private int _xpEarned = 1234;
    private int _communityLevel = 1234;
    private int _xpLeft = 1234;
    private int _beatmapsTracked = 1234;
    private int _mbUsed = 1234;
    private int _playerAge = 1234;
    public int Users { get => _users; set => this.RaiseAndSetIfChanged(ref _users, value);}
    public int Translators { get => _translators; set => this.RaiseAndSetIfChanged(ref _translators, value); }
    public int SongsPlayed { get => _songsPlayed; set => this.RaiseAndSetIfChanged(ref _songsPlayed, value);}
    public int XpEarned { get => _xpEarned; set => this.RaiseAndSetIfChanged(ref _xpEarned, value);}
    public int CommunityLevel { get => _communityLevel; set => this.RaiseAndSetIfChanged(ref _communityLevel, value);}
    public int XpLeft { get => _xpLeft; set => this.RaiseAndSetIfChanged(ref _xpLeft, value);}
    public int BeatmapsTracked { get => _beatmapsTracked; set => this.RaiseAndSetIfChanged(ref _beatmapsTracked, value);}
    public int MbUsed { get => _mbUsed; set => this.RaiseAndSetIfChanged(ref _mbUsed, value);}
    public int PlayerAge { get => _playerAge; set => this.RaiseAndSetIfChanged(ref _playerAge, value);}

    private void Block(CompositeDisposable disposable)
    {
        
    }
}