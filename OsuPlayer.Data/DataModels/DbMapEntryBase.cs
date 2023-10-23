using System.Text;
using Nein.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;

namespace OsuPlayer.Data.DataModels;

/// <summary>
/// a minimal beatmap entry with only frequently used data
/// </summary>
public class DbMapEntryBase : IMapEntryBase
{
    public required IDbReaderFactory DbReaderFactory { get; init; }

    public long DbOffset { get; init; }
    public string? OsuPath { get; init; }
    public string Artist { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Hash { get; init; } = string.Empty;
    public int BeatmapSetId { get; init; }
    public int TotalTime { get; init; }
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();
    public string SongName => GetSongName();
    public string ArtistString => GetArtist();
    public string TitleString => GetTitle();

    /// <summary>
    /// Gets the artist
    /// <remarks>may be overridden for usage with <see cref="DbMapEntry.UseUnicode" /></remarks>
    /// </summary>
    /// <returns>the artist</returns>
    public virtual string GetArtist()
    {
        return Artist;
    }

    /// <summary>
    /// Gets the title
    /// <remarks>may be overridden for usage with <see cref="DbMapEntry.UseUnicode" /></remarks>
    /// </summary>
    /// <returns>the title</returns>
    public virtual string GetTitle()
    {
        return Title;
    }

    public string GetSongName()
    {
        return $"{GetArtist()} - {GetTitle()}";
    }

    /// <summary>
    /// Reads a osu!.db map entry and fills a full <see cref="DbMapEntry" /> with data
    /// </summary>
    /// <returns>a new <see cref="DbMapEntry" /> generated from osu!.db data</returns>
    public IMapEntry? ReadFullEntry()
    {
        if (OsuPath == null) return null;

        var reader = GetReader();

        if (reader == default)
            return null;

        return reader.ReadFullEntry(OsuPath, this, dbOffset: DbOffset);
    }

    public IDatabaseReader? GetReader()
    {
        if (OsuPath == null) 
            return null;

        return DbReaderFactory.CreateDatabaseReader(OsuPath);
    }

    public bool Equals(IMapEntryBase? other)
    {
        return Hash == other?.Hash;
    }

    public int CompareTo(IMapEntryBase? other)
    {
        return string.Compare(Hash, other?.Hash, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return GetSongName();
    }

    public static bool operator ==(DbMapEntryBase? left, IMapEntryBase? right)
    {
        return left?.Hash == right?.Hash;
    }

    public static bool operator !=(DbMapEntryBase? left, IMapEntryBase? right)
    {
        return left?.Hash != right?.Hash;
    }

    public override bool Equals(object? other)
    {
        if (other is IMapEntryBase map)
            return Hash == map.Hash;

        return false;
    }

    public override int GetHashCode()
    {
        return BitConverter.ToInt32(Encoding.UTF8.GetBytes(Hash));
    }
}