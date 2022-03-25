using System.Globalization;
using Avalonia.Data.Converters;
using DynamicData;
using OsuPlayer.Data.OsuPlayer.Classes;

namespace OsuPlayer.Extensions.ValueConverters;

public class SourceListValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == default)
            return new List<Playlist>();

        if (value is SourceList<Playlist> val)
        {
            return val.Items.ToList();
        }

        return new List<Playlist>();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}