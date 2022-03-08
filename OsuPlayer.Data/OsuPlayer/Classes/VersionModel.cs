namespace OsuPlayer.Data.OsuPlayer.Classes;

public sealed class VersionModel
{
    public DateTime Version { get; set; }

    public new string ToString()
    {
        return Version.ToString("yyyy.Mdd.Hmm.s");
    }
}