using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

namespace OsuPlayer.Extensions.ValueConverters;

public class IsCurrentSongOnBlacklistConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var x = (bool) value;

        return x ? MaterialIconKind.HeartBroken : MaterialIconKind.HeartBrokenOutline;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}