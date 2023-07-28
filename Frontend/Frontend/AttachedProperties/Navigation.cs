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
using Frontend.Views;

namespace Frontend.AttachedProperties;

public class Navigation : AvaloniaObject
{
    static Navigation()
    {
        HeaderProperty.Changed.AddClassHandler<Button>(HandleHeaderChanged);
    }

    public static readonly AttachedProperty<string?> HeaderProperty =
        AvaloniaProperty.RegisterAttached<Navigation, Button, string?> ("Header", default, false, Avalonia.Data.BindingMode.OneWay);

    public static void SetHeader(AvaloniaObject element, string value)
    {
        element.SetValue(HeaderProperty, value);
    }
    public static string? GetHeader(AvaloniaObject element)
    {
        return element.GetValue(HeaderProperty);
    }

    private static void HandleHeaderChanged(AvaloniaObject target, AvaloniaPropertyChangedEventArgs args)
    {
    }
}
