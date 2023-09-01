using Avalonia.Media;
using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Extensions.EnumExtensions;

public static class KnownColorsExtensions
{
    public static Color ToColor(this KnownColors color)
    {
        return Color.FromUInt32((uint) color);
    }
}