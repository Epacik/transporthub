using Autofac;
using Avalonia.Controls;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using TransportHub.AttachedProperties;
using TransportHub.Common;
using TransportHub.Services;
using TransportHub.Api;
using TransportHub.Views;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TransportHub.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IPageFactory _pageFactory;
    private readonly IDialogService _dialogService;
    private readonly ISystemInfoService _systemInfoService;
    private readonly Stack<(string Route, Control Page, Dictionary<string, object>? parameters)> _navigationStack = new();

    #region properties

    [ObservableProperty]
    public bool _showPane = false;
    public bool ShowBackButton => _navigationStack.Count >= 2;
    public double TitleMargin => (_navigationStack.Count >= 2, ShowPane) switch
    {
        (true, true) => 88,
        (true, false) or (false, true) => 44,
        _ => 0
    };
    public bool ShowTitle => Content is not LoginView && _systemInfoService.IsDesktop;
    public Control? Header => Content is not null ? Navigation.GetHeader(Content) : null;

    [ObservableProperty]
    public Control? _content;

    partial void OnContentChanged(Control? value)
    {
        OnPropertyChanged(nameof(Header));
        OnPropertyChanged(nameof(ShowTitle));
        HeaderChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion properties

    public event EventHandler HeaderChanged;

    public MainViewModel(
        IAuthorizationService authorizationService,
        IPageFactory pageFactory,
        IDialogService dialogService,
        ISystemInfoService systemInfoService)
    {
        _authorizationService = authorizationService;
        _pageFactory = pageFactory;
        _dialogService = dialogService;
        _systemInfoService = systemInfoService;
        _authorizationService.LoggedIn += AuthorizationService_Authorized;

        NavigateToAsync(Routes.Login, null)
            .ContinueWith(async t =>
            {
                if (t.IsFaulted)
                {
                    await _dialogService.ShowAlertAsync("Error", t.Exception?.Message ?? "");
                }
            });
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

        _navigationStack.Push((next.Route, next.Page, parameters));
        UpdateHeaderAndBackButton();
    }

    internal async Task NavigateToAsync(string route, Dictionary<string, object>? parameters)
    {
        if(_navigationStack.TryPeek(out var old) &&
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

        _navigationStack.Push((route, page, parameters));
        UpdateHeaderAndBackButton();
    }

    private void UpdateHeaderAndBackButton()
    {
        OnPropertyChanged(nameof(ShowBackButton));
        OnPropertyChanged(nameof(TitleMargin));
    }
}
