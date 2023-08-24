using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;
using TransportHub.Core.AttachedProperties;

namespace TransportHub.Core.Converters;

public class ViewToRouteConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    { 
        throw new NotSupportedException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not UserControl control)
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return Navigation.GetRoute(control);
    }
}
