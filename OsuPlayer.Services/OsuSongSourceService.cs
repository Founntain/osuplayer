﻿using System.Collections.ObjectModel;
using DynamicData;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.IO.Importer;

namespace OsuPlayer.Services;

public class OsuSongSourceService : OsuPlayerService, ISongSourceProvider
{
    private readonly ReadOnlyObservableCollection<IMapEntryBase>? _songSourceList;

    public SourceList<IMapEntryBase> SongSource { get; } = new();
    public IObservable<IChangeSet<IMapEntryBase>>? Songs { get; }
    public ReadOnlyObservableCollection<IMapEntryBase>? SongSourceList => _songSourceList;

    public override string ServiceName => "OSU_SONGSOURCE_SERVICE";

    public OsuSongSourceService(ISortProvider? sortProvider = null)
    {
        if (sortProvider != null)
        {
            sortProvider.SortedSongs = SongSource.Connect().Sort(sortProvider.SortingModeObservable);

            Songs = sortProvider.SortedSongs;

            Songs.Bind(out _songSourceList).Subscribe();
        }
        else
        {
            Songs = SongSource.Connect();

            Songs.Bind(out _songSourceList).Subscribe();
        }
    }

    public IMapEntryBase? GetMapEntryFromHash(string? hash)
    {
        return SongSourceList!.FirstOrDefault(x => x.Hash == hash);
    }

    public List<IMapEntryBase> GetMapEntriesFromHash(ICollection<string> hashes, out ICollection<string> invalidHashes)
    {
        var maps = hashes.Select(x => SongSourceList!.FirstOrDefault(map => map.Hash == x)).ToArray();

        invalidHashes = new List<string>();

        for (var i = 0; i < maps.Length; i++)
            if (maps[i] == null)
                invalidHashes.Add(hashes.ElementAt(i));

        return maps.Where(map => map != null).ToList();
    }
}