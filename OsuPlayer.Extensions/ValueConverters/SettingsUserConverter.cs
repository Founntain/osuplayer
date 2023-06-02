using System.Globalization;
using Avalonia.Data.Converters;
using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Extensions.ValueConverters;

public class SettingsUserConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is UserModel user)
            return user.Name;

        return "Not logged in";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}