using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;
using Material.Icons.Avalonia;

namespace OsuPlayer.Extensions.ValueConverters;

public class RepeatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool val)
        {
            return val ? MaterialIconKind.Repeat : MaterialIconKind.RepeatOff;
        }

        return MaterialIconKind.QuestionMark;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}