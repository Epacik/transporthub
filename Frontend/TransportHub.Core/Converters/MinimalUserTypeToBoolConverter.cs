using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;

namespace TransportHub.Core.Converters;

public class MinimalUserTypeToBoolConverter : IValueConverter, IMultiValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is UserType v && parameter is UserType p)
        {
            return ((int)v) >= ((int)p);
        }
        return true;
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return true;

        if (values[0] is UserType v && values[1] is UserType p)
        {
            return ((int)v) >= ((int)p);
        }
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
