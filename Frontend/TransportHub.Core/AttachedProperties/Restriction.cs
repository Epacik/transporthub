using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;

namespace TransportHub.Core.AttachedProperties;

public class Restriction : AvaloniaObject
{
    public static readonly AttachedProperty<UserType?> MinimalUserTypeProperty =
        AvaloniaProperty.RegisterAttached<Navigation, Control, UserType?>("MinimalUserType", inherits: true);

    public static void SetMinimalUserType(Control element, UserType? minimalUserType)
    {
        element.SetValue(MinimalUserTypeProperty, minimalUserType);
    }

    public static UserType? GetMinimalUserType(Control element)
    {
        return element.GetValue(MinimalUserTypeProperty);
    }


    public static event EventHandler<UserType?>? CurrentUserTypeChanged;
    public static readonly AttachedProperty<UserType?> CurrentUserTypeProperty =
        AvaloniaProperty.RegisterAttached<Navigation, Control, UserType?>(
            "CurrentUserType",
            inherits: true,
            coerce: (ctr, val) =>
            {
                CurrentUserTypeChanged?.Invoke(ctr, val);
                return val;
            });

    public static void SetCurrentUserType(Control element, UserType? currentUserType)
    {
        element.SetValue(CurrentUserTypeProperty, currentUserType);
    }

    public static UserType? GetCurrentUserType(Control element)
    {
        return element.GetValue(CurrentUserTypeProperty);
    }
}
