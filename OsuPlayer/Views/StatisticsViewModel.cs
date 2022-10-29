using System.Reactive.Disposables;
using OsuPlayer.Base.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class StatisticsViewModel : BaseViewModel
{
    private int _beatmapsTracked = 1234;
    private uint _communityLevel = 1234;
    private float _mbUsed = 1234f;
    private string _playerAge = "1234";
    private ulong _songsPlayed = 1234;
    private uint _translators = 1234;

    private uint _users = 1234;
    private ulong _xpEarned = 1234;
    private ulong _xpLeft = 1234;

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

    private void Block(CompositeDisposable disposable)
    {
        // done separately so they can fail independently
        UpdateApiStatistics();
        UpdateBeatmapCount();
        UpdateStorageAmount();
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

    private async void UpdateApiStatistics()
    {
        var statistics = await ApiAsync.GetApiStatistics();
        Users = statistics.TotalUserCount;
        Translators = statistics.TranslatorCount;
        XpEarned = statistics.CommunityTotalXp;
        CommunityLevel = statistics.CommunityLevel;
        XpLeft = statistics.CommunityXpLeft;
    }

    private async void UpdateStorageAmount()
    {
        MbUsed = await ApiAsync.GetStorageAmount();
    }

    private async void UpdateBeatmapCount()
    {
        BeatmapsTracked = await ApiAsync.GetBeatmapsCount();
    }
}