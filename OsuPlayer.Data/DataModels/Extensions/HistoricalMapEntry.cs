using OsuPlayer.Data.DataModels.Interfaces;

namespace OsuPlayer.Data.DataModels.Extensions;

public class HistoricalMapEntry : IComparable<HistoricalMapEntry>
{
    public IMapEntryBase MapEntry { get; }
    public DateTimeOffset TimePlayed { get; }

    public string TimePlayedString => $"Last Played: {TimePlayed:G}";

    public HistoricalMapEntry(IMapEntryBase mapEntry) : this(mapEntry, DateTimeOffset.Now)
    {
    }

    public HistoricalMapEntry(IMapEntryBase mapEntry, DateTimeOffset timePlayed)
    {
        MapEntry = mapEntry;
        TimePlayed = timePlayed;
    }

    public int CompareTo(HistoricalMapEntry other)
    {
        return TimePlayed.CompareTo(other.TimePlayed);
    }

    public override bool Equals(object? obj)
    {
        if (obj is HistoricalMapEntry other)
        {
            return MapEntry.Hash == other.MapEntry.Hash;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return MapEntry.GetHashCode();
    }
}