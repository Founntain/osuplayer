using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;
using Material.Icons.Avalonia;
using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Extensions.ValueConverters;

public class RepeatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RepeatMode val)
        {
            if (targetType == typeof(MaterialIconKind))
                switch (val)
                {
                    case RepeatMode.NoRepeat:
                        return MaterialIconKind.RepeatOff;
                    case RepeatMode.Playlist:
                        return MaterialIconKind.Repeat;
                    case RepeatMode.SingleSong:
                        return MaterialIconKind.RepeatOnce;
                    default:
                        return MaterialIconKind.RepeatOff;
                }

            if (targetType == typeof(bool) && val == RepeatMode.Playlist)
                return true;
            return false;
        }

        return MaterialIconKind.QuestionMark;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}