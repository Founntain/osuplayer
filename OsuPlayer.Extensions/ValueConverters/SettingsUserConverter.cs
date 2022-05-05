using System.Globalization;
using Avalonia.Data.Converters;
using OsuPlayer.Data.API.Models.User;

namespace OsuPlayer.Extensions.ValueConverters;

public class SettingsUserConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null && value is UserModel user)
            return user.Name;
        
        return "Not logged in";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}