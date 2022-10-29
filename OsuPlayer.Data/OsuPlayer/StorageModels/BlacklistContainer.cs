// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace OsuPlayer.Data.OsuPlayer.StorageModels;

public class BlacklistContainer : IStorableContainer
{
    public HashSet<string> Songs { get; set; } = new();

    public IStorableContainer Init()
    {
        return this;
    }
}