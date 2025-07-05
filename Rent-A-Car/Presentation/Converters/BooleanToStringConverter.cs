using System;
using System.Globalization;
using System.Windows.Data;

namespace Rent_A_Car.Presentation.Converters;

public class BooleanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool available)
        {
            return available ? "Да" : "Нет";
        }
        return "Неизвестно";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}