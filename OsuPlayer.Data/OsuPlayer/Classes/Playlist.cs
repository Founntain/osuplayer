using System.ComponentModel;

namespace OsuPlayer.Data.OsuPlayer.Classes;

public class Playlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public DateTime CreationTime { get; } = DateTime.UtcNow;
    public BindingList<string> Songs { get; set; } = new ();

    public override string ToString() => Name;
    
    public static bool operator ==(Playlist? left, Playlist? right)
    {
        return left?.Id == right?.Id;
    }

    public static bool operator !=(Playlist? left, Playlist? right)
    {
        return left?.Id != right?.Id;
    }
}