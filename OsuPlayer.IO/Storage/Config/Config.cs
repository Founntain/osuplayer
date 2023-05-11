using OsuPlayer.Data.OsuPlayer.StorageModels;

namespace OsuPlayer.IO.Storage.Config;

public class Config : Storable<ConfigContainer>
{
    private static ConfigContainer? _configContainer;
    public override string Path => System.IO.Path.Combine("data", "config.json");

    public Config()
    {
        _configContainer ??= Read();

        Container = _configContainer;
    }
}