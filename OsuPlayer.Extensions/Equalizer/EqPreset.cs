namespace OsuPlayer.Extensions.Equalizer;

/// <summary>
/// Object for an equalizer preset. Contained in <see cref="OsuPlayer.Extensions.Equalizer.EqPreset" />
/// </summary>
public class EqPreset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public decimal[] Gain { get; set; } = new decimal[10];

    public static EqPreset Flat { get; } = new()
    {
        Name = "Flat (Default)",
        Gain = new decimal[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        }
    };

    public static EqPreset Custom { get; set; } = new()
    {
        Name = "Custom",
        Gain = new decimal[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        }
    };

    public static EqPreset Classic { get; } = new()
    {
        Name = "Classic",
        Gain = new decimal[]
        {
            0, 0, 0, 0, 0, 0, 0, -2, -3, -4
        }
    };

    public static EqPreset LaptopSpeaker { get; } = new()
    {
        Name = "Laptop speaker",
        Gain = new decimal[]
        {
            2, 6, 2, -2, -1, 0, 2, 3, 5, 8
        }
    };

    public override string ToString()
    {
        return Name;
    }
}