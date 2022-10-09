using System.Globalization;
using Avalonia.Data.Converters;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Importer;
using Splat;

namespace OsuPlayer.ValueConverters;

/// <summary>
/// This ValueConverter is in the OsuPlayer Project, because we need access to the Audio class
/// to convert the checksum to a MapEntry
/// </summary>
public class PlaylistValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == default) return default;

        return Locator.Current.GetRequiredService<ISongSourceProvider>().GetMapEntriesFromHash((ICollection<string>) value);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}