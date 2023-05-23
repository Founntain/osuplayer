using OsuPlayer.Data.OsuPlayer.StorageModels;

namespace OsuPlayer.IO.Storage.Equalizer;

public class EqStorage : Storable<EqContainer>
{
    private static EqContainer? _eqContainer;

    public override string Path => System.IO.Path.Combine("data", "eqPresets.json");

    public EqStorage()
    {
        _eqContainer ??= Read();

        Container = _eqContainer;
    }
}