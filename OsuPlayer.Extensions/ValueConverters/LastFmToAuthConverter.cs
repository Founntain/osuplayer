using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace OsuPlayer.Extensions.ValueConverters;

public class LastFmToAuthConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isAuthorized = value != null && (bool) value;

        return isAuthorized 
            ? new SolidColorBrush(Color.FromRgb(0, 200, 0))
            : new SolidColorBrush(Color.FromRgb(255, 204, 34));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}