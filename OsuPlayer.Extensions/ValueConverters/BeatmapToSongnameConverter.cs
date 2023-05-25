using System.Globalization;
using Avalonia.Data.Converters;
using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Extensions.ValueConverters;

public class BeatmapToSongnameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == default) return string.Empty;
        
        var beatmap = value as BeatmapModel;
        
        if (beatmap == default) return string.Empty;

        return $"{beatmap.Artist} - {beatmap.Title}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}