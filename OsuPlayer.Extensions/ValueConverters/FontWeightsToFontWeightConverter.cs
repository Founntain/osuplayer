using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Extensions.ValueConverters;

public class FontWeightsToFontWeightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var val = (FontWeights) value;

        return (FontWeight) val;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var val = (FontWeight) value;

        return (FontWeights) val;
    }
}