using Avalonia.Threading;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.IO.Storage.Playlists;
using Splat;

namespace OsuPlayer.IO.Importer;

public static class CollectionImporter
{
    /// <summary>
    /// Imports the collections found in the osu! collection.db and adds them as playlists
    /// </summary>
    /// <param name="source">a <see cref="ISongSource"/> to perform the collection import on</param>
    /// <returns>a bool indicating import success</returns>
    public static async Task<bool> ImportCollectionsAsync(ISongSource source)
    {
        var config = new Config();

        var reader = source.SongSourceList?[0].GetReader(config.Container.OsuPath!);

        if (reader == null) return false;

        var collections = await reader.GetCollections(config.Container.OsuPath!);

        if (collections != default && collections.Any())
        {
            var beatmapHashes = await reader.GetBeatmapHashes();

            foreach (var collection in collections)
            foreach (var hash in collection.BeatmapHashes)
            {
                var setId = beatmapHashes?.GetValueOrDefault(hash) ?? -1;
                await PlaylistManager.AddSongToPlaylistAsync(collection.Name, source.SongSourceList.FirstOrDefault(x => x.BeatmapSetId == setId)?.Hash ?? string.Empty);
            }

            return true;
        }

        return false;
    }
}