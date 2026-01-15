using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace avaloniaCrossPlat.Converters;

public class StringToBrushConverter : IValueConverter
{
    public static readonly StringToBrushConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            if (value is null)
                return null;
            string[] array = {
                "#0a9396",
                "#94d2bd",
                "#e9d8a6",
                "#ee9b00",
                "#ca6702",
                "#bb3e03",
                "#ae2012",
                "#9b2226"
                };
                
            if (Decimal.TryParse((value.ToString() ?? "").Replace('.',','), out var decValue))
            {
                if (decValue < 0)
                    return new SolidColorBrush(Color.Parse(array[1]));
                if (decValue < 10)
                    return new SolidColorBrush(Color.Parse(array[2]));
                if (decValue < 30)
                    return new SolidColorBrush(Color.Parse(array[3]));
                if (decValue < 45)
                    return new SolidColorBrush(Color.Parse(array[4]));
                if (decValue < 54)
                    return new SolidColorBrush(Color.Parse(array[5]));
                if (decValue < 60)
                    return new SolidColorBrush(Color.Parse(array[6]));
                else
                    return new SolidColorBrush(Color.Parse(array[7]));
            }
        }
        catch
        {
            return null;
        }
        return null;
    }


    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
