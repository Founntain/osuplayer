using OsuPlayer.Data.OsuPlayer.StorageModels;

namespace OsuPlayer.IO.Storage.Equalizer;

public class EqStorage : Storable<EqContainer>
{
    public override string Path => System.IO.Path.Combine("data", "eqPresets.json");
}