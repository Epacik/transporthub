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
    [ObservableProperty]
    private string? _login;

    [ObservableProperty]
    private string? _password;

    public LoginViewModel(
        IAuthorizationService authorizationService,
        INavigationService navigationService)
    {
        _authorizationService = authorizationService;
        _navigationService = navigationService;
    }

    public async Task OpenStartupSettings()
    {
        await _navigationService.NavigateToAsync(Routes.StartupSettings);
    }
}
