using System.Globalization;
using Avalonia.Data.Converters;
using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Extensions.ValueConverters;

public class ClientsToAmountConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Checks if Values[0] is a Guid and assigns it to hostId, same for clients.
        if (value is not HashSet<UserModel> clients || clients.Count == 0) 
            return "Unkown host";

        return $"{clients.Count} people listening" ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}