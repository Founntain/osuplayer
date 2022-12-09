using System.Globalization;
using Avalonia.Data.Converters;
using DynamicData;
using OsuPlayer.Data.OsuPlayer.StorageModels;

namespace OsuPlayer.Extensions.ValueConverters;

public class SourceListValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            null => new List<Playlist>(),
            SourceList<Playlist> val => val.Items.ToList(),
            _ => new List<Playlist>()
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}