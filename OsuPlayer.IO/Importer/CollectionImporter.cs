using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.IO.Storage.Playlists;

namespace OsuPlayer.IO.Importer;

public static class CollectionImporter
{
    /// <summary>
    /// Imports the collections found in the osu! collection.db and adds them as playlists
    /// </summary>
    /// <param name="sourceProvider">a <see cref="ISongSourceProvider"/> to perform the collection import on</param>
    /// <returns>a bool indicating import success</returns>
    public static async Task<bool> ImportCollectionsAsync(ISongSourceProvider sourceProvider)
    {
        var config = new Config();

        var reader = sourceProvider.SongSourceList?[0].GetReader(config.Container.OsuPath!);

        if (reader == null) return false;

        var collections = await reader.GetCollections(config.Container.OsuPath!);

        if (collections != default && collections.Any())
        {
            var beatmapHashes = reader.GetBeatmapHashes();

            await using var playlistStorage = new PlaylistStorage();
            await playlistStorage.ReadAsync();

            foreach (var collection in collections)
            foreach (var hash in collection.BeatmapHashes)
            {
                var setId = beatmapHashes.GetValueOrDefault(hash);
                var songHash = sourceProvider.SongSourceList.FirstOrDefault(x => x.BeatmapSetId == setId)?.Hash ?? string.Empty;

                if (playlistStorage.Container.Playlists?.FirstOrDefault(x => x.Name == collection.Name) is { } playlist)
                {
                    playlist.Songs.Add(songHash);
                    continue;
                }

                playlist = new Playlist
                {
                    Name = collection.Name
                };
                playlist.Songs.Add(songHash);

                playlistStorage.Container.Playlists?.Add(playlist);
            }

            return true;
        }

        return false;
    }
}