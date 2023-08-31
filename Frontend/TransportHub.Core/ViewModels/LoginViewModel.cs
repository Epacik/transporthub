using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TransportHub.Common;
using TransportHub.Services;
using TransportHub.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using TransportHub.Core.Services;

namespace TransportHub.Core.ViewModels;

public partial class LoginViewModel : ObservableObject, INavigationAware
{
    private readonly IAuthorizationService _authorizationService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly ILogger _logger;
    private readonly ILoadingPopupService _loadingPopupService;

    [ObservableProperty]
    private string? _login;

    [ObservableProperty]
    private string? _password;

    [ObservableProperty]
    private bool _enableLogin = true;

    public LoginViewModel(
        IAuthorizationService authorizationService,
        INavigationService navigationService,
        IDialogService dialogService,
        ILogger logger,
        ILoadingPopupService loadingPopupService)
    {
        _authorizationService = authorizationService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _logger = logger;
        _loadingPopupService = loadingPopupService;
        if (logger.IsEnabled(LogEventLevel.Information))
        {
            logger.Information("Opening Login page");
        }
    }

    public async Task OpenStartupSettings()
    {
        await _navigationService.NavigateToAsync(Routes.StartupSettings);
    }

    public async Task LoginUser()
    {
        try
        {
            await _loadingPopupService.Show("Logowanie");
            EnableLogin = false;
            var result = await _authorizationService.Login(Login, Password, true);

            if (result.IsError)
            {
                await _dialogService.ShowAlertAsync("Błąd logowania", result.UnwrapErr().Message);
                return;
            }

            var isLoggedIn = result.Unwrap();

            //if (!isLoggedIn)
            //{
            //    await _dialogService.ShowAlertAsync("Błąd logowania", "Logowanie nie powiodło się");
            //    return;
            //}

            await _navigationService.NavigateToAsync(Routes.Dashboard);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occured");
            throw;
        }
        finally
        {
            EnableLogin = true;
            await _loadingPopupService.Hide();
        }
    }

    public Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        return Task.FromResult(true);
    }

    public async Task OnNavigatedFrom()
    {
        await Task.Yield();
        Login = "";
        Password = "";
    }
}
