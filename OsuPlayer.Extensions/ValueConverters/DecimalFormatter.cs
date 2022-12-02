using System.Globalization;
using Avalonia.Data.Converters;

namespace OsuPlayer.Extensions.ValueConverters;

public class DecimalFormatter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType != typeof(string)) return null;

        if (value is not IFormattable number) return "";

        var format = number.ToString("N0", CultureInfo.CurrentCulture);

        return format;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}