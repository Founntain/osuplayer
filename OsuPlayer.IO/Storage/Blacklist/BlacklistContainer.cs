// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System.ComponentModel;

namespace OsuPlayer.IO.Storage.Blacklist;

public class BlacklistContainer : IStorableContainer
{
    public BindingList<string> Songs { get; set; } = new();

    public IStorableContainer Init()
    {
        return this;
    }
}