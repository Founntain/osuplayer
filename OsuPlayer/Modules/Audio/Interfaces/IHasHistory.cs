using System.Collections;
using OsuPlayer.IO.DbReader.DataModels.Extensions;

namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface provides historic capabilities
/// </summary>
public interface IHasHistory
{
    public BindableList<HistoricalMapEntry> History { get; }
}