using Autofac;
using Avalonia.Controls;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using TransportHub.Core.AttachedProperties;
using TransportHub.Common;
using TransportHub.Services;
using TransportHub.Api;
using TransportHub.Core.Views;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using TransportHub.Core.Assets.Icons;
using TransportHub.Core.Models;
using TransportHub.Core.Services;
using Avalonia.Threading;
using System.Linq;

namespace TransportHub.Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IPageFactory _pageFactory;
    private readonly IDialogService _dialogService;
    private readonly ISystemInfoService _systemInfoService;
    private readonly IOnScreenKeyboardService _onScreenKeyboardService;
    private readonly Stack<(string Route, Control Page, Dictionary<string, object>? parameters)> _navigationStack = new();

    private bool _disableOnSelectedNavItemChanged = false;
    #region properties

    [ObservableProperty]
    public bool _showPane = false;
    public bool ShowBackButton => _navigationStack.Count >= 2;
    //public NavigationViewPaneDisplayMode PaneDisplayMode => ShowPane ? NavigationViewPaneDisplayMode.Auto : NavigationViewPaneDisplayMode.LeftMinimal;
    public double TitleMargin => (_navigationStack.Count >= 2 && !VerticalButtons, ShowPane) switch
    {
        (true, true) => 88,
        (true, false) or (false, true) => 44,
        _ => 0
    };
    public bool ShowTitle => Content is not LoginView && _systemInfoService.IsDesktop;
    public Control? Header => Content is not null ? Navigation.GetHeader(Content) : null;

    [ObservableProperty]
    private Control? _content;

    [ObservableProperty]
    private NavItem? _selectedNavItem;

    partial void OnSelectedNavItemChanged(NavItem? value)
    {
        if (_disableOnSelectedNavItemChanged)
            return;

        Task.Run(async () => await NavigateToAsync(value?.Route, null, false))
            .ContinueWith(t =>
            {
            });
    }

    [ObservableProperty]
    private NavItem[] _navItems =
    {
        new("Przegląd", Tabler.Home, Routes.Dashboard),
        new("Zamówienia", Tabler.TruckDelivery, Routes.Orders)
    };

    [ObservableProperty]
    private NavItem[] _footerItems =
    {
        new("Użytkownicy", Tabler.Users, Routes.Users),
        new("Ustawienia", Tabler.Settings, Routes.Settings),
    };

    partial void OnContentChanged(Control? value)
    {
        OnPropertyChanged(nameof(Header));
        OnPropertyChanged(nameof(ShowTitle));
        HeaderChanged?.Invoke(this, EventArgs.Empty);
    }

    [ObservableProperty]
    private Thickness _contentMargin;

    [ObservableProperty]
    private bool _verticalButtons;

    partial void OnVerticalButtonsChanged(bool value)
    {
        UpdateHeaderAndBackButton();
    }

    #endregion properties

    public event EventHandler? HeaderChanged;

    public MainViewModel(
        IAuthorizationService authorizationService,
        IPageFactory pageFactory,
        IDialogService dialogService,
        ISystemInfoService systemInfoService,
        IOnScreenKeyboardService onScreenKeyboardService)
    {
        _authorizationService = authorizationService;
        _pageFactory = pageFactory;
        _dialogService = dialogService;
        _systemInfoService = systemInfoService;
        _onScreenKeyboardService = onScreenKeyboardService;
        _authorizationService.LoggedIn += AuthorizationService_Authorized;
        _onScreenKeyboardService.HeightChanged += OnScreenKeyboardService_HeightChanged;

        NavigateToAsync(Routes.Login, null)
            .ContinueWith(async t =>
            {
                if (t.IsFaulted)
                {
                    await _dialogService.ShowAlertAsync("Error", t.Exception?.Message ?? "");
                }
            });
    }

    private void OnScreenKeyboardService_HeightChanged(double obj)
    {
        ContentMargin = new Thickness(0, 0, 0, obj);
    }

    private void AuthorizationService_Authorized()
    {
        ShowPane = true;
        _navigationStack.Clear();
        UpdateHeaderAndBackButton();
    }

    internal async Task NavigateBackAsync(Dictionary<string, object>? parameters)
    {
        if (_navigationStack.Count < 2)
            return;

        var current = _navigationStack.Pop();
        var next = _navigationStack.Pop();

        if (current.Page.DataContext is INavigationAwareViewModel currentModel)
        {
            await currentModel.OnNavigatedFrom();
        }

        Content = next.Page;

        if (Content.DataContext is INavigationAwareViewModel model)
        {
            await model.OnNavigatedTo(parameters ?? next.parameters);
        }

        SetSelectedNavItem(next.Route);

        _navigationStack.Push((next.Route, next.Page, parameters));
        UpdateHeaderAndBackButton();
    }

    internal Task NavigateToAsync(string route, Dictionary<string, object>? parameters, bool updateMenu = true)
    {
        return Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_navigationStack.TryPeek(out var old) &&
           old.Page.DataContext is INavigationAwareViewModel oldModel)
            {
                await oldModel.OnNavigatedFrom();
            }

            var page = _pageFactory
                .GetPage(route)
                .Match(
                    page => page,
                    err => _pageFactory.GetInvalidPage(err.Message));

            Content = page;

            if (page.DataContext is INavigationAwareViewModel model)
            {
                await model.OnNavigatedTo(parameters);
            }

            if(updateMenu)
            {
                SetSelectedNavItem(route);
            }

            _navigationStack.Push((route, page, parameters));
            UpdateHeaderAndBackButton();
        });
    }

    private void SetSelectedNavItem(string route)
    {
        _disableOnSelectedNavItemChanged = true;

        var item = NavItems.FirstOrDefault(x => route.StartsWith(x.Route));
        item ??= FooterItems.FirstOrDefault(x => route.StartsWith(x.Route));

        SelectedNavItem = item;

        _disableOnSelectedNavItemChanged = false;
    }

    private void UpdateHeaderAndBackButton()
    {
        OnPropertyChanged(nameof(ShowBackButton));
        //OnPropertyChanged(nameof(PaneDisplayMode));
        OnPropertyChanged(nameof(TitleMargin));
    }
}
