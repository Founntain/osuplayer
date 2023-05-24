using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using OsuPlayer.Data.API.Models.User;

namespace OsuPlayer.Extensions.ValueConverters;

public class HostIdToHostNameConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2) return string.Empty;

        if (values[0] == AvaloniaProperty.UnsetValue || values[0] == default || 
            values[1] == AvaloniaProperty.UnsetValue || values[1] == default)
            return string.Empty;

        // Checks if Values[0] is a Guid and assigns it to hostId, same for clients.
        if ((values[0] is not Guid hostId) || (values[1] is not ISet<UserModel> clients) || clients.Count == 0) 
            return "Unkown host";

        return clients.FirstOrDefault(x => x.Id == hostId)?.Name ?? "Unkown host";
    }
}