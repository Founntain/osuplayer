namespace OsuPlayer.Extensions.Equalizer;

/// <summary>
///     Object for an equalizer preset. Contained in <see cref="EqPresetStorage" />
/// </summary>
public class EqPreset
{
    public readonly int Id;
    public readonly string Name;
    public double[] Gain;

    /// <param name="name">Name of the preset</param>
    /// <param name="value">Gain values of the eq-bands in dB (10 double values, Range -15 to +15)</param>
    public EqPreset(int id, string name, double[] value)
    {
        Id = id;
        Name = name;
        Gain = value;
    }


    public static EqPreset Flat { get; } = new(0, "Flat (Default)", new double[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0});

    public static EqPreset Custom { get; set; } = new(1, "Custom", new double[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0});

    public static EqPreset Classical { get; } = new(3, "Classical", new double[] {0, 0, 0, 0, 0, 0, 0, -2, -3, -4});

    public static EqPreset LaptopSpeaker { get; } =
        new(4, "Laptop speaker", new double[] {2, 6, 2, -2, -1, 0, 2, 3, 5, 8});

    public override string ToString()
    {
        return Name;
    }
}