using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using TransportHub.Extensions;
using TransportHub.Api;
using TransportHub.ViewModels;
using System;

namespace TransportHub.Views;

public partial class MainView : UserControl
{
    private readonly IAuthorizationService _authorizationService;

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    public MainView(IAuthorizationService authorizationService)
    {
        InitializeComponent();
        nvSample.BackRequested += NvSample_BackRequested;
        _authorizationService = authorizationService;
        _authorizationService.LoggedIn += AuthorizationService_Authorized;
    }

    private void AuthorizationService_Authorized()
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
