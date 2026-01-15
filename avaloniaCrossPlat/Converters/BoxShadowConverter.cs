using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace avaloniaCrossPlat.Converters;

public class BoxShadowConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BoxShadow shadow)
        {
            return shadow;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}