using System.Collections.ObjectModel;
using DynamicData;

namespace OsuPlayer.IO.Playlists;

public class Playlist
{
    public string Name { get; set; }
    public ObservableCollection<string> Songs { get; set; } = new ();

    public override string ToString() => Name;
}