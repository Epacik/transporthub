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
using TransportHub.Api.Dtos;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using TransportHub.Core.Mappers;

namespace TransportHub.Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IPageFactory _pageFactory;
    private readonly IDialogService _dialogService;
    private readonly ISystemInfoService _systemInfoService;
    private readonly IOnScreenKeyboardService _onScreenKeyboardService;
    private readonly IUsersService _usersService;
    private readonly IReportErrorService _reportErrorService;
    public ILoggedInUserService LoggedInUserService { get; }
    private readonly Stack<(string Route, Control Page, Dictionary<string, object>? parameters)> _navigationStack = new();

    private string? _currentRoute;
    private bool _disableOnSelectedNavItemChanged = false;
    #region properties

    [ObservableProperty]
    public bool _showPane = false;
    public bool ShowBackButton => _navigationStack.Count >= 2;
    //public NavigationViewPaneDisplayMode PaneDisplayMode => ShowPane ? NavigationViewPaneDisplayMode.Auto : NavigationViewPaneDisplayMode.LeftMinimal;
    public double TitleMargin =>
        ((_navigationStack.Count >= 2 && !VerticalButtons) || _currentRoute == Routes.StartupSettings, ShowPane) switch
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

    partial void OnSelectedNavItemChanged(NavItem? oldValue, NavItem? newValue)
    {
        if (_disableOnSelectedNavItemChanged)
            return;

        if (string.IsNullOrEmpty(newValue?.Route))
        {
            _disableOnSelectedNavItemChanged = true;
            SelectedNavItem = oldValue;
            _disableOnSelectedNavItemChanged = false;

            Task.Run(() => _dialogService.ShowConfirmation("Wyloguj się", "Czy chcesz się wylogować?"))
                .ContinueWith(async t =>
                {
                    if (t.IsCompleted && t.Result)
                    {
                        _ = await _authorizationService.Logout();
                    }
                });
            return;
        }

        Task.Run(async () => await NavigateToAsync(newValue?.Route, null, false))
            .ContinueWith(t =>
            {
            });
    }

    [ObservableProperty]
    private NavItem[] _navItems =
    {
        new("Przegląd", Tabler.IconHome, Routes.Dashboard, UserType.User),
        new("Zamówienia", Tabler.IconTruckDelivery, Routes.Orders, UserType.User)
    };

    [ObservableProperty]
    private NavItem[] _footerItems =
    {
        new("Administruj", Tabler.IconFileSettings, Routes.Administer, UserType.Manager),
        new("Ustawienia", Tabler.IconSettings, Routes.Settings, UserType.User),
        new("Wyloguj", Tabler.IconLogout, "", UserType.User),
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

    [ObservableProperty]
    private bool _loadingVisible;

    [ObservableProperty]
    private string? _loadingMessage;

    #endregion properties

    public event EventHandler? HeaderChanged;

    public MainViewModel(
        IAuthorizationService authorizationService,
        IPageFactory pageFactory,
        IDialogService dialogService,
        ISystemInfoService systemInfoService,
        IOnScreenKeyboardService onScreenKeyboardService,
        IUsersService usersService,
        IReportErrorService reportErrorService,
        ILoggedInUserService loggedInUserService)
    {
        _authorizationService = authorizationService;
        _pageFactory = pageFactory;
        _dialogService = dialogService;
        _systemInfoService = systemInfoService;
        _onScreenKeyboardService = onScreenKeyboardService;
        _usersService = usersService;
        _reportErrorService = reportErrorService;
        LoggedInUserService = loggedInUserService;
        _authorizationService.LoggedIn += AuthorizationService_LoggedIn;
        _authorizationService.LoggedOut += AuthorizationService_LoggedOut;
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

    private async void AuthorizationService_LoggedOut()
    {
        ShowPane = false;
        _navigationStack.Clear();
        UpdateHeaderAndBackButton();
        await NavigateToAsync(Routes.Login, null);
    }

    private void OnScreenKeyboardService_HeightChanged(double obj)
    {
        ContentMargin = new Thickness(0, 0, 0, obj);
    }

    private void AuthorizationService_LoggedIn(LoginResponseDto responseDto)
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

        if (current.Page is INavigationAware oldPage)
        {
            await oldPage.OnNavigatedFrom();
        }

        if (current.Page.DataContext is INavigationAware currentModel)
        {
            await currentModel.OnNavigatedFrom();
        }


        Content = next.Page;
        _currentRoute = next.Route;

        SetSelectedNavItem(next.Route);

        _navigationStack.Push((next.Route, next.Page, parameters));
        UpdateHeaderAndBackButton();

        if (Content is INavigationAware page)
        {
            await page.OnNavigatedTo(parameters ?? next.parameters);
        }

        if (Content.DataContext is INavigationAware model)
        {
            await model.OnNavigatedTo(parameters ?? next.parameters);
        }
    }

    internal Task NavigateToAsync(string route, Dictionary<string, object>? parameters, bool updateMenu = true)
    {
        return Dispatcher.UIThread.InvokeAsync(async () =>
        {
            

            if (_navigationStack.TryPeek(out var old))
            {
                if (old.Page is INavigationAware oldPage)
                {
                    await oldPage.OnNavigatedFrom();
                }

                if (old.Page.DataContext is INavigationAware oldModel)
                {
                    await oldModel.OnNavigatedFrom();
                }


                if (old.Route == Routes.Login && route != Routes.StartupSettings)
                {
                    _navigationStack.Clear();

                    if (_authorizationService.IsLoggedIn)
                    {
                        await LoggedInUserService.ForceRefreshAsync();
                    }
                }

                if (old.Route == route)
                {
                    return;
                }
            }

            var page = _pageFactory
                .GetPage(route)
                .Match(
                    page => page,
                    err => _pageFactory.GetInvalidPage(err.ToStringDemystified()));

            Content = page;
            _currentRoute = route;

            if (updateMenu)
            {
                SetSelectedNavItem(route);
            }

            _navigationStack.Push((route, page, parameters));
            UpdateHeaderAndBackButton();

            if (Content is INavigationAware newPage)
            {
                await newPage.OnNavigatedTo(parameters);
            }

            if (page.DataContext is INavigationAware model)
            {
                await model.OnNavigatedTo(parameters);
            }
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
        OnPropertyChanged(nameof(ShowPane));
        OnPropertyChanged(nameof(TitleMargin));
    }
}
