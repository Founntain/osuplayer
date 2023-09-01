using System.Globalization;
using Avalonia.Data.Converters;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Extensions.ObjectExtensions;

namespace OsuPlayer.Extensions.ValueConverters;

public class BeatmapModelFullSongNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not BeatmapModel beatmapModel ? string.Empty : beatmapModel.FullSongName();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}