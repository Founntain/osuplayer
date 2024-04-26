using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.OsuPlayer.StorageModels;

namespace OsuPlayer.IO.Storage.Blacklist;

public sealed class Blacklist : Storable<BlacklistContainer>
{
    private static BlacklistContainer? _blacklistContainer;

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