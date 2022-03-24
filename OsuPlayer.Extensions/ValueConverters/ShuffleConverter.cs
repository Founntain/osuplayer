using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

namespace OsuPlayer.Extensions.ValueConverters;

public class ShuffleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool val)
        {
            return val ? MaterialIconKind.Shuffle : MaterialIconKind.ShuffleDisabled;
        }

        return MaterialIconKind.QuestionMark;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}