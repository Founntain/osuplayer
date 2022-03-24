using System.Globalization;
using Avalonia.Data.Converters;

namespace OsuPlayer.Extensions.ValueConverters;

public class GridFormatter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double or int) return 0;
        var width = System.Convert.ToDouble(value);

        if (parameter is not double or int) return 0;
        var targetWidth = System.Convert.ToDouble(parameter);

        if (targetWidth <= 0) return 0;
        return (int) Math.Ceiling(width / targetWidth);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}