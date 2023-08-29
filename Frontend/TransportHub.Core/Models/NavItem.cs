using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TransportHub.Api;

namespace TransportHub.Core.Models;

public partial class NavItem : ObservableObject
{

    [ObservableProperty]
    private string _text;

    [ObservableProperty]
    private string _icon;

    [ObservableProperty]
    private string _route;

    [ObservableProperty]
    private UserType _minimalUserType;

    public NavItem(string text, string icon, string route, UserType minimalUserType)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Icon = icon ?? throw new ArgumentNullException(nameof(icon));
        Route = route ?? throw new ArgumentNullException(nameof(route));
        MinimalUserType = minimalUserType;
    }
}
