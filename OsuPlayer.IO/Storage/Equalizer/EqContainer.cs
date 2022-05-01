using OsuPlayer.Data.OsuPlayer.Classes;

namespace OsuPlayer.IO.Storage.Equalizer;

public class EqContainer : IStorableContainer
{
    public decimal[] LastUsedEqParams { get; set; } = new decimal[10];

    public List<EqPreset>? EqPresets { get; set; }

    public EqContainer Init()
    {
        EqPresets ??= new List<EqPreset>();
        EqPresets.Add(EqPreset.Flat);
        EqPresets.Add(EqPreset.Custom);
        EqPresets.Add(EqPreset.Classic);
        EqPresets.Add(EqPreset.LaptopSpeaker);

        return this;
    }
}