using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace avaloniaCrossPlat.Converters;

public class TrendToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool trend)
        {
            if (trend)
                return StreamGeometry.Parse("M11 18 H13 V9 L16.5 12.5 L18 11 L12 5 L6 11 L7.5 12.5 L11 9 Z");
            else return StreamGeometry.Parse("M11 6 H13 V15 L16.5 11.5 L18 13 L12 19 L6 13 L7.5 11.5 L11 15 Z");
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}