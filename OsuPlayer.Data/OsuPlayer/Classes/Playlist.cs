using System.Collections.ObjectModel;
using System.Xml.Xsl;

namespace OsuPlayer.Data.OsuPlayer.Classes;

public class Playlist
{
    public string Name { get; set; }
    public ObservableCollection<string> Songs { get; set; } = new ();

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