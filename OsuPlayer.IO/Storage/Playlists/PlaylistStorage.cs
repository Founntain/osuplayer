using Newtonsoft.Json;
using OsuPlayer.Data.OsuPlayer.StorageModels;

namespace OsuPlayer.IO.Storage.Playlists;

/// <summary>
/// The PlaylistStorage reads and writes playlist data to the playlists.json file located in the data folder.
/// </summary>
public class PlaylistStorage : Storable<PlaylistContainer>
{
    private static PlaylistContainer? _playlistContainer;

    protected override JsonSerializerSettings SerializerSettings { get; } = new()
    {
        Formatting = Formatting.None
    };

    public override string Path => System.IO.Path.Combine("data", "playlists.json");

    public PlaylistStorage()
    {
        _playlistContainer ??= Read();

        Container = _playlistContainer;
    }
}