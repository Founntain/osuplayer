using System.Globalization;
using Avalonia.Data.Converters;

namespace OsuPlayer.Extensions.ValueConverters;

public class GridFormatter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var width = System.Convert.ToDouble(value);
        var targetWidth = System.Convert.ToDouble(parameter);

        return (int)Math.Ceiling(width / targetWidth);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}