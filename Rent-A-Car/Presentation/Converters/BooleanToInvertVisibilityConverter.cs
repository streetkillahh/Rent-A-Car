using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rent_A_Car.Presentation.Converters;

public class BooleanToInvertVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool available)
        {
            return available ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}