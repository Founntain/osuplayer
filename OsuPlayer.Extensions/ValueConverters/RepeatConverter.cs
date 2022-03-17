﻿using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

namespace OsuPlayer.Extensions.ValueConverters;

public class RepeatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var val = (bool) value!;
        return val ? MaterialIconKind.Repeat : MaterialIconKind.RepeatOff;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}