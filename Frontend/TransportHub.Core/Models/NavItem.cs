using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace TransportHub.Core.Models;

public partial class NavItem : ObservableObject
{

    [ObservableProperty]
    private string _text;

    [ObservableProperty]
    public string _icon;

    [ObservableProperty]
    public string _route;

    public NavItem(string text, string icon, string route)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Icon = icon ?? throw new ArgumentNullException(nameof(icon));
        Route = route ?? throw new ArgumentNullException(nameof(route));
    }
}
