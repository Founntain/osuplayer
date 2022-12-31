using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using LiveChartsCore.Defaults;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Network.API.Service.Endpoints;
using Splat;

namespace OsuPlayer.Modules.Services;

public class ApiStatisticsProvider : IStatisticsProvider
{
    public BindableList<ObservableValue> GraphValues { get; } = new();
    public event PropertyChangedEventHandler? UserDataChanged;

    public async Task UpdateOnlineStatus(UserOnlineStatusType statusType, string? song = null, string? checksum = null)
    {
        await Locator.Current.GetService<NorthFox>().SetUserOnlineStatus(statusType, song, checksum);
    }

    public async Task UpdateXp(string hash, double timeListened, double songLength)
    {
        if (ProfileManager.User == default) return;

        var currentTotalXp = ProfileManager.User.TotalXp;

        var time = timeListened / 1000;

        var response = await ApiAsync.UpdateXpFromCurrentUserAsync(
            hash,
            time,
            songLength);

        if (response == default) return;

        ProfileManager.User = response;

        var xpEarned = response.TotalXp - currentTotalXp;

        GraphValues.Add(new ObservableValue(xpEarned));

        await Dispatcher.UIThread.InvokeAsync(() => UserDataChanged?.Invoke(this, new PropertyChangedEventArgs("Xp")));
    }

    public async Task UpdateSongsPlayed(int beatmapSetId)
    {
        if (ProfileManager.User == default) return;

        var response = await ApiAsync.UpdateSongsPlayedForCurrentUserAsync(1, beatmapSetId);

        if (response == default) return;

        ProfileManager.User = response;

        await Dispatcher.UIThread.InvokeAsync(() => UserDataChanged?.Invoke(this, new PropertyChangedEventArgs("SongsPlayed")));
    }
}