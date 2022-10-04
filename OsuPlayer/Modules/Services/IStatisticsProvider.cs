using System.ComponentModel;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using OsuPlayer.Data.API.Enums;

namespace OsuPlayer.Modules.Services;

public interface IStatisticsProvider
{
    public BindableList<ObservableValue> GraphValues { get; }

    public event PropertyChangedEventHandler? UserDataChanged;

    /// <summary>
    /// Updates the online status of the user.
    /// </summary>
    /// <param name="statusType">The <see cref="UserOnlineStatusType"/> to be set</param>
    /// <param name="song">the currently playing song name</param>
    /// <param name="checksum">the md5 hash of the playing song</param>
    /// <returns></returns>
    public Task UpdateOnlineStatus(UserOnlineStatusType statusType, string? song = null, string? checksum = null);

    /// <summary>
    /// Updates the user's xp.
    /// </summary>
    /// <param name="hash">The md5 hash of the song which gave the xp</param>
    /// <param name="timeListened">Time listened in milliseconds</param>
    /// <param name="songLength">The total song length in seconds</param>
    /// <returns></returns>
    public Task UpdateXp(string hash, double timeListened, double songLength);

    /// <summary>
    /// Updates the amount of times the user has played a song.
    /// </summary>
    /// <param name="beatmapSetId">The beatmap set id of the song played</param>
    /// <returns></returns>
    public Task UpdateSongsPlayed(int beatmapSetId);
}