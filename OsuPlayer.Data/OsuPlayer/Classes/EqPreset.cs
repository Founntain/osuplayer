namespace OsuPlayer.Data.OsuPlayer.Classes;

/// <summary>
/// Object for an equalizer preset. Contained in <see cref="EqPreset" />
/// </summary>
public class EqPreset
{
    public Guid Id { get; init; } = Guid.NewGuid();
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

    public override bool Equals(object? obj)
    {
        if (obj is not EqPreset preset) return false;

        return Id.Equals(preset.Id);
    }

    public static bool operator ==(EqPreset? left, EqPreset? right)
    {
        return left?.Id == right?.Id;
    }

    public static bool operator !=(EqPreset? left, EqPreset? right)
    {
        return left?.Id != right?.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return Name;
    }
}