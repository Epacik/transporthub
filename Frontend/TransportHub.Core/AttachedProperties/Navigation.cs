using Avalonia;
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TransportHub.Core.Views;
using FluentAvalonia.UI.Controls;

namespace TransportHub.Core.AttachedProperties;

public class Navigation : AvaloniaObject
{
    static Navigation()
    {
        
    }

    public static readonly AttachedProperty<Control?> HeaderProperty =
        AvaloniaProperty.RegisterAttached<Navigation, Button, Control?> ("Header", default, false, Avalonia.Data.BindingMode.OneWay);

    public static void SetHeader(AvaloniaObject element, Control value)
    {
        element.SetValue(HeaderProperty, value);
    }
    public static Control? GetHeader(AvaloniaObject element)
    {
        return element.GetValue(HeaderProperty);
    }

    public static readonly AttachedProperty<string?> RouteProperty =
        AvaloniaProperty.RegisterAttached<Navigation, Control, string?>("Route", inherits: true);

    public static void SetRoute(Control element, string? route)
    {
        element.SetValue(RouteProperty, route);
    }

    public static string? GetRoute(Control element)
    {
        return element.GetValue(RouteProperty);
    }
}
