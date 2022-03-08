using System.Collections.Generic;

namespace OsuPlayer.Modules.Extensions.Equalizer;

/// <summary>
///     Container for the <see cref="Equalizer.EqPreset" />
/// </summary>
public sealed class EqPresetStorage
{
    public static double F80 { get; set; }
    public static double F125 { get; set; }
    public static double F200 { get; set; }
    public static double F300 { get; set; }
    public static double F500 { get; set; }
    public static double F1000 { get; set; }
    public static double F2000 { get; set; }
    public static double F4000 { get; set; }
    public static double F8000 { get; set; }
    public static double F16000 { get; set; }

    public double[] CurrentPreset
    {
        get => new[] {F80, F125, F200, F300, F500, F1000, F2000, F4000, F8000, F16000};
        set
        {
            F80 = value[0];
            F125 = value[1];
            F200 = value[2];
            F300 = value[3];
            F500 = value[4];
            F1000 = value[5];
            F2000 = value[6];
            F4000 = value[7];
            F8000 = value[8];
            F16000 = value[9];
        }
    }

    public List<EqPreset> EqPreset { get; set; } = new()
    {
        Equalizer.EqPreset.Flat
    };
}