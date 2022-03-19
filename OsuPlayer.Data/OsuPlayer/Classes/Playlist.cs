using System.ComponentModel;

namespace OsuPlayer.Data.OsuPlayer.Classes;

public class Playlist
{
    public string Name { get; set; }
    public DateTime CreationTime { get; } = DateTime.UtcNow;
    public BindingList<string> Songs { get; set; } = new ();

    public override string ToString() => Name;
    
    public static bool operator ==(Playlist? left, Playlist? right)
    {
        return left?.Name == right?.Name;
    }

    public static bool operator !=(Playlist? left, Playlist? right)
    {
        return left?.Name != right?.Name;
    }
}