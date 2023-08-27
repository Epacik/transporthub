using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Converters;

public class ObjectToGridLengthConverter : IValueConverter
{
    private static readonly GridLength _gridLengthZero = new(0);
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return _gridLengthZero;

        return parameter as GridLength? ?? GridLength.Star;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
