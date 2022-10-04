using System.ComponentModel;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using OsuPlayer.Data.API.Enums;

namespace OsuPlayer.Modules.Services;

public interface IStatisticsProvider
{
    public BindableList<ObservableValue> GraphValues { get; }

    public event PropertyChangedEventHandler? UserDataChanged;

    public Task UpdateOnlineStatus(UserOnlineStatusType statusType, string? song = null, string? checksum = null);
    public Task UpdateXp(string hash, double timeListened, double songLength);
    public Task UpdateSongsPlayed(int beatmapSetId);
}