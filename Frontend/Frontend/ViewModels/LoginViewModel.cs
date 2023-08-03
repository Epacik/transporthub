using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Helpers;
using Frontend.Services;
using Frontend.Services.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthorizationService _authorizationService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    [ObservableProperty]
    private string? _login;

    [ObservableProperty]
    private string? _password;

    public LoginViewModel(
        IAuthorizationService authorizationService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _authorizationService = authorizationService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    public async Task OpenStartupSettings()
    {
        await _navigationService.NavigateToAsync(Routes.StartupSettings);
    }

    public async Task LoginUser()
    {
        var result = await _authorizationService.Authorize(Login, Password, true);

        if (result.IsError)
        {
            await _dialogService.ShowAlertAsync("Błąd logowania", result.UnwrapErr().Message);
            return;
        }

        var isLoggedIn = result.Unwrap();

        if (!isLoggedIn)
        {
            await _dialogService.ShowAlertAsync("Błąd logowania", "Logowanie nie powiodło się");
            return;
        }

        await _navigationService.NavigateToAsync(Routes.Dashboard);
    }
}
