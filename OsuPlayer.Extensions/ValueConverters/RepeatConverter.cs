using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;
using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Extensions.ValueConverters;

public class RepeatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not RepeatMode val) return MaterialIconKind.QuestionMark;
        
        if (targetType == typeof(MaterialIconKind))
        {
            return val switch
            {
                RepeatMode.NoRepeat => MaterialIconKind.RepeatOff,
                RepeatMode.Playlist => MaterialIconKind.Repeat,
                RepeatMode.SingleSong => MaterialIconKind.RepeatOnce,
                _ => MaterialIconKind.RepeatOff
            };
        }

        return targetType == typeof(bool) && val == RepeatMode.Playlist;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}