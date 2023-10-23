﻿using System.ComponentModel;
using Avalonia.Threading;
using LiveChartsCore.Defaults;
using Nein.Extensions;
using Nein.Extensions.Bindables;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.Enums;
using OsuPlayer.Api.Data.API.RequestModels.User;
using OsuPlayer.Interfaces.Service;
using Splat;

namespace OsuPlayer.Services;

public class ApiStatisticsService : OsuPlayerService, IStatisticsProvider
{
    public BindableList<ObservableValue> GraphValues { get; } = new();

    public override string ServiceName => "API_STATISTICS_SERVICE";
    public event PropertyChangedEventHandler? UserDataChanged;

    public async Task UpdateOnlineStatus(UserOnlineStatusType statusType, string? song = null, string? checksum = null)
    {
        if (ProfileManager.User == default || ProfileManager.User.UniqueId == Guid.Empty)
            return;

        await Locator.Current.GetRequiredService<NorthFox>().User.SetOnlineStatus(new UserOnlineStatusModel
        {
            StatusType = statusType,
            Song = song,
            SongChecksum = checksum
        });
    }

    public async Task UpdateXp(string hash, double timeListened, double channelLength)
    {
        if (ProfileManager.User == default) return;

        var currentTotalXp = ProfileManager.User.TotalXp;

        var time = timeListened / 1000;

        var response = await Locator.Current.GetRequiredService<NorthFox>().User.UpdateXp(new UpdateXpModel
        {
            SongChecksum = hash,
            ChannelLength = channelLength,
            ElapsedMilliseconds = time
        });

        if (response == default) return;

        ProfileManager.User = response.ConvertObjectToJson<User>();

        var xpEarned = response.TotalXp - currentTotalXp;

        GraphValues.Add(new ObservableValue(xpEarned));

        await Dispatcher.UIThread.InvokeAsync(() => UserDataChanged?.Invoke(this, new PropertyChangedEventArgs("Xp")));

        LogToConsole($"Successfully updated XP for {ProfileManager.User?.Name ?? string.Empty}. Earned {xpEarned} XP.");
    }

    public async Task UpdateSongsPlayed(int beatmapSetId)
    {
        if (ProfileManager.User == default) return;

        var response = await Locator.Current.GetRequiredService<NorthFox>().User.UpdateSongsPlayed(1, beatmapSetId);

        if (response == default) return;

        ProfileManager.User = response.ConvertObjectToJson<User>();

        await Dispatcher.UIThread.InvokeAsync(() => UserDataChanged?.Invoke(this, new PropertyChangedEventArgs("SongsPlayed")));

        LogToConsole($"Successfully updated songs played for {ProfileManager.User?.Name ?? string.Empty}");
    }
}