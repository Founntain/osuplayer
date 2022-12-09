using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

namespace OsuPlayer.Extensions.ValueConverters;

public class VolumeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double val) return MaterialIconKind.QuestionMark;

        return val switch
        {
            0 => MaterialIconKind.VolumeMute,
            < 33 => MaterialIconKind.VolumeLow,
            < 66 => MaterialIconKind.VolumeMedium,
            _ => MaterialIconKind.VolumeHigh
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}