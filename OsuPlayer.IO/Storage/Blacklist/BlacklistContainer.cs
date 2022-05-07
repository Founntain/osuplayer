namespace OsuPlayer.IO.Storage.Blacklist;

public class BlacklistContainer : IStorableContainer
{
    public List<string>? Songs { get; set; }

    public IStorableContainer Init()
    {
        Songs = new List<string>();

        return this;
    }
}