using Avalonia.Media;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;

namespace OsuPlayer.Extensions;

public static class FontWeightExtensions
{
    public static FontWeights GetNextBiggerFont(this ConfigContainer container)
    {
        return container.DefaultFontWeight.GetNextBiggerFont();
    }

    public static FontWeights GetNextBiggerFont(this FontWeights font)
    {
        var fontSizes = (FontWeights[]) Enum.GetValues(typeof(FontWeights));

        var i = Array.IndexOf(fontSizes, font) + 1;
        return fontSizes.Length == i ? fontSizes[i - 1] : fontSizes[i];
    }

    public static FontWeights GetNextSmallerFont(this ConfigContainer container)
    {
        return container.DefaultFontWeight.GetNextSmallerFont();
    }

    public static FontWeights GetNextSmallerFont(this FontWeights font)
    {
        var fontSizes = (FontWeights[]) Enum.GetValues(typeof(FontWeights));

        var i = Array.IndexOf(fontSizes, font) - 1;
        return i == -1 ? fontSizes[i + 1] : fontSizes[i];
    }

    public static FontWeight ToFontWeight(this FontWeights font)
    {
        return font switch
        {
            FontWeights.Black => FontWeight.Black,
            FontWeights.Thin => FontWeight.Thin,
            FontWeights.ExtraLight => FontWeight.ExtraLight,
            FontWeights.Light => FontWeight.Light,
            FontWeights.Regular => FontWeight.Regular,
            FontWeights.Medium => FontWeight.Medium,
            FontWeights.SemiBold => FontWeight.SemiBold,
            FontWeights.Bold => FontWeight.Bold,
            FontWeights.ExtraBold => FontWeight.ExtraBold,
            _ => FontWeight.Regular
        };
    }
}