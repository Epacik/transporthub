using Avalonia.Data.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Converters;

public class IdToItemConverter : IValueConverter, IMultiValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not IEnumerable enumerable || value is not string id)
            return value;

        foreach (var item in enumerable)
        {
            var type = item.GetType();
            var idProp = type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);

            if (idProp is null)
                continue;

            var getter = idProp.GetGetMethod(false) ?? idProp.GetGetMethod(true);

            if (getter is null || getter.ReturnType != typeof(string))
                continue;
            var val = getter.Invoke(item, null) as string;

            if (val == id)
                return item;
        }


        return null;
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return "";

        var enumerable = values.FirstOrDefault(x => x is IEnumerable && x is not string) as IEnumerable;
        var id = values.FirstOrDefault(x => x is string) as string;

        if (enumerable is null || id is null)
            return "";

        foreach (var item in enumerable)
        {
            var type = item.GetType();
            var idProp = type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);

            if (idProp is null)
                continue;

            var getter = idProp.GetGetMethod(false) ?? idProp.GetGetMethod(true);

            if (getter is null || getter.ReturnType != typeof(string))
                continue;
            var val = getter.Invoke(item, null) as string;

            if (val == id)
                return item;
        }


        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
