

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace OsuPlayer.Data.OsuPlayer.StorageModels;

/// <summary>
/// A playlist object containing an <see cref="Id" />, the <see cref="Name" />, the <see cref="CreationTime" /> and a
/// <see cref="Songs" /> list
/// </summary>
public class Playlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Contains the hashes of the songs in the <see cref="Playlist" />
    /// </summary>
    public HashSet<string> Songs { get; set; } = new();

    private bool Equals(Playlist other)
    {
        return Id.Equals(other.Id) && Name == other.Name && CreationTime.Equals(other.CreationTime) &&
               Songs.SequenceEqual(other.Songs);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Playlist) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, CreationTime, Songs);
    }

    public override string ToString()
    {
        return Name;
    }

    public static bool operator ==(Playlist? left, Playlist? right)
    {
        return left?.Id == right?.Id;
    }

    public static bool operator !=(Playlist? left, Playlist? right)
    {
        return left?.Id != right?.Id;
    }
}