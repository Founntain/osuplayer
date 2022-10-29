using System.Reactive.Disposables;
using System.Threading.Tasks;
using OsuPlayer.Base.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class StatisticsViewModel : BaseViewModel
{
    private int _beatmapsTracked;
    private uint _communityLevel;
    private float _mbUsed;
    private string _playerAge;
    private ulong _songsPlayed;
    private uint _translators;

    private uint _users;
    private ulong _xpEarned;
    private ulong _xpLeft;

    public uint Users
    {
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }

    public uint Translators
    {
        get => _translators;
        set => this.RaiseAndSetIfChanged(ref _translators, value);
    }

    public ulong SongsPlayed
    {
        get => _songsPlayed;
        set => this.RaiseAndSetIfChanged(ref _songsPlayed, value);
    }

    public ulong XpEarned
    {
        get => _xpEarned;
        set => this.RaiseAndSetIfChanged(ref _xpEarned, value);
    }

    public uint CommunityLevel
    {
        get => _communityLevel;
        set => this.RaiseAndSetIfChanged(ref _communityLevel, value);
    }

    public ulong XpLeft
    {
        get => _xpLeft;
        set => this.RaiseAndSetIfChanged(ref _xpLeft, value);
    }

    public int BeatmapsTracked
    {
        get => _beatmapsTracked;
        set => this.RaiseAndSetIfChanged(ref _beatmapsTracked, value);
    }

    public float MbUsed
    {
        get => _mbUsed;
        set => this.RaiseAndSetIfChanged(ref _mbUsed, value);
    }

    public string PlayerAge
    {
        get => _playerAge;
        set => this.RaiseAndSetIfChanged(ref _playerAge, value);
    }

    public StatisticsViewModel()
    {
        Activator = new ViewModelActivator();

        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposable)
    {
        // Done separately so they can fail independently
        await UpdateApiStatistics();
        await UpdateBeatmapCount();
        await UpdateStorageAmount();
        
        UpdateDate();
    }

    private void UpdateDate()
    {
        var timeSince = DateTime.Now.Subtract(new DateTime(2017, 11, 1));

        var time = DateTime.MinValue + timeSince;

        // note: MinValue is 1/1/1 so we have to subtract...
        var years = time.Year - 1;
        var months = time.Month - 1;
        var days = time.Day - 1;

        PlayerAge = $"{years} years, {months} months, {days} days, {timeSince.Hours}:{timeSince.Minutes}:{timeSince.Seconds} old";
    }

    private async Task UpdateApiStatistics()
    {
        var statistics = await ApiAsync.GetApiStatistics();
        
        Users = statistics.TotalUserCount;
        Translators = statistics.TranslatorCount;
        XpEarned = statistics.CommunityTotalXp;
        CommunityLevel = statistics.CommunityLevel;
        XpLeft = statistics.CommunityXpLeft;
    }

    private async Task UpdateStorageAmount()
    {
        MbUsed = await ApiAsync.GetStorageAmount();
    }

    private async Task UpdateBeatmapCount()
    {
        BeatmapsTracked = await ApiAsync.GetBeatmapsCount();
    }
}