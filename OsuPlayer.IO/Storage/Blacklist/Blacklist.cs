﻿using Newtonsoft.Json;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.OsuPlayer.StorageModels;

namespace OsuPlayer.IO.Storage.Blacklist;

public class Blacklist : Storable<BlacklistContainer>
{
    private static BlacklistContainer? _blacklistContainer;

    protected override JsonSerializerSettings SerializerSettings { get; } = new()
    {
        Formatting = Formatting.None
    };

    public override string? Path => System.IO.Path.Combine("data", "blacklist.json");

    public Blacklist()
    {
        _blacklistContainer ??= Read();

        Container = _blacklistContainer;
    }

    public bool Contains(IMapEntryBase? map)
    {
        return Container.Songs.Contains(map?.Hash);
    }
}