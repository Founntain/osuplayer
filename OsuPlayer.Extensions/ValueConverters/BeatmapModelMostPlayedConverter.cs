using System.Globalization;
using Avalonia.Data.Converters;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Extensions.ObjectExtensions;

namespace OsuPlayer.Extensions.ValueConverters;

public class BeatmapModelMostPlayedConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not BeatmapModel beatmapModel ? string.Empty : beatmapModel.MostPlayedString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}