using Newtonsoft.Json;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Storage.Blacklist;

public class Blacklist : Storable<BlacklistContainer>
{
    protected override JsonSerializerSettings SerializerSettings { get; } = new()
    {
        Formatting = Formatting.None
    };

    public override string? Path => System.IO.Path.Combine("data", "blacklist.json");

    public bool Contains(IMapEntryBase? map)
    {
        return Container.Songs.Contains(map?.Hash);
    }
}