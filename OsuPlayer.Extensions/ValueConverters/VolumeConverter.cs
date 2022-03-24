using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;
using Material.Icons.Avalonia;

namespace OsuPlayer.Extensions.ValueConverters;

public class VolumeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double val)
        {
            if (val == 0)
                return MaterialIconKind.VolumeMute;
            if (val < 33)
                return MaterialIconKind.VolumeLow;
            if (val < 66)
                return MaterialIconKind.VolumeMedium;
            return MaterialIconKind.VolumeHigh;
        }

        return MaterialIconKind.QuestionMark;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}