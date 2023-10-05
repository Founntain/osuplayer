namespace OsuPlayer.IO.DbReader.DataModels.Extensions;

public class HistoricalMapEntryComparer : IEqualityComparer<HistoricalMapEntry>
{
    public bool Equals(HistoricalMapEntry? x, HistoricalMapEntry? y)
    {
        if(x == null && y == null) return true;
        if(x == null || y == null) return false;
        
        return x.MapEntry.Hash == y.MapEntry.Hash;
    }

    public int GetHashCode(HistoricalMapEntry obj)
    {
        return obj.MapEntry.GetHashCode();
    }
}