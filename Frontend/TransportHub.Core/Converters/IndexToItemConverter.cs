using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Converters;

public class IndexToItemConverter : IValueConverter, IMultiValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not IEnumerable<string> enumerable || value is not int index)
            return value;

        var list = enumerable.ToList()!;

        return list[index];
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return "";

        var enumerable = values.FirstOrDefault(x => x is IEnumerable<string>) as IEnumerable<string>;
        var index = values.FirstOrDefault(x => x is int) as int?;

        if (enumerable is null || index is null)
            return "";

        return enumerable.ToList()[(int)index];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
