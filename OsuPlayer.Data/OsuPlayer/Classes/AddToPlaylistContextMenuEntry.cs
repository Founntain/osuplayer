namespace OsuPlayer.Data.OsuPlayer.Classes;

public class AddToPlaylistContextMenuEntry
{
    public string Name { get; set; }
    public Action<string> Action { get; set; }

    public AddToPlaylistContextMenuEntry(string name, Action<string> action)
    {
        Name = name;
        Action = action;
    }
}