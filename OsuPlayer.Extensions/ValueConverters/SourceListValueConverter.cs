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

        if (value.GetType() == typeof(SourceList<>))
            return new List<Playlist>();
        
        var list = (SourceList<Playlist>) value;

        return list.Items.ToList();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}