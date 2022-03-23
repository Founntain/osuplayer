using System.ComponentModel;

namespace OsuPlayer.Data.OsuPlayer.Classes;

public class Playlist
{
    protected bool Equals(Playlist other)
    {
        return Id.Equals(other.Id) && Name == other.Name && CreationTime.Equals(other.CreationTime) &&
               Songs.Equals(other.Songs);
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

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public DateTime CreationTime { get; } = DateTime.UtcNow;
    public BindingList<string> Songs { get; set; } = new();

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