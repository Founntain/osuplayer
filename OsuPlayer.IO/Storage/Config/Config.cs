namespace OsuPlayer.IO.Storage.Config;

public class Config : Storable<ConfigContainer>
{
    public override string Path => System.IO.Path.Combine("data", "config.json");
}