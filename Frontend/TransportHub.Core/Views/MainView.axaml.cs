using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using TransportHub.Core.Extensions;
using TransportHub.Api;
using TransportHub.Core.ViewModels;
using System;
using System.ComponentModel;
using TransportHub.Api.Dtos;

namespace TransportHub.Core.Views;

public partial class MainView : UserControl
{
    private readonly IAuthorizationService _authorizationService;

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    public MainView(IAuthorizationService authorizationService)
    {
        InitializeComponent();
        Navigation.BackRequested += NvSample_BackRequested;
        Navigation.PaneClosed += (s, e) => ResetIconSize();
        Navigation.PaneOpened += (s, e) => ResetIconSize();
        _authorizationService = authorizationService;
        _authorizationService.LoggedIn += AuthorizationService_Authorized;
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        if (ViewModel != null )
        {
            ViewModel.VerticalButtons = e.NewSize.Width >= 640;
        } 
    }

    private void Navigation_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        var prop = e.Property.Name;
        if(prop == nameof(NavigationView.MenuItemsSource) || prop == nameof(NavigationView.FooterMenuItemsSource))
        {
            ResetIconSize();
        }
    }

    private void ResetIconSize()
    {
        var desc = Navigation.FindVisualDescendants<Viewbox>();

        if (desc.IsOk)
        {
            foreach (var box in desc.Unwrap())
            {
                box.Height = 24;
            }
        }
    }

    private void AuthorizationService_Authorized(LoginResponseDto responseDto)
    {

        var borderResult = this.FindVisualDescendant<Border>("ContentGridBorder");

        if (borderResult is null)
            return;

        if (borderResult.IsError)
            return;

        var border = borderResult.Unwrap();

        if (border is null)
            return;

        border.Margin = new Thickness(0, 44, 0, 0);
    }

    private async void NvSample_BackRequested(object? sender, NavigationViewBackRequestedEventArgs e)
    {
        if (ViewModel is null)
            return;

        await ViewModel.NavigateBackAsync(null);
    }
}
