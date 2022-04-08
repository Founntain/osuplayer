using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

namespace OsuPlayer.Extensions.ValueConverters;

public class IsCurrentSongInPlaylistConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var x = (bool) value;

        return x ? MaterialIconKind.Heart : MaterialIconKind.HeartOutline;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}