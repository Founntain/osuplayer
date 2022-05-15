using Newtonsoft.Json;

namespace OsuPlayer.IO.Storage.Playlists;

/// <summary>
/// The PlaylistStorage reads and writes playlist data to the playlists.json file located in the data folder.
/// </summary>
public class PlaylistStorage : Storable<PlaylistContainer>
{
    protected override JsonSerializerSettings SerializerSettings { get; } = new()
    {
        Formatting = Formatting.None
    };

    public override string Path => System.IO.Path.Combine("data", "playlists.json");
}