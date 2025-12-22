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
        if (value is string s && !string.IsNullOrWhiteSpace(s))
        {
            try
            {
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
                if (Decimal.TryParse(s, out var decValue))
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
                    else
                        return new SolidColorBrush(Color.Parse(array[6]));
                }
                return new SolidColorBrush(Color.Parse(s));
            }
            catch
            {
                // couleur invalide
            }
        }

        return Brushes.Transparent; // fallback
    }


    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
