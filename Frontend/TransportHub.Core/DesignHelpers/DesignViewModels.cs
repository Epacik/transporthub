using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Core.Services;
using TransportHub.Core.Services.Impl;
using TransportHub.Core.Services.Impl.Empty;
using TransportHub.Core.Services.InMemory.API;
using TransportHub.Core.ViewModels;
using TransportHub.Services.InMemory.API;

namespace TransportHub.Core.DesignHelpers;

public static class DesignViewModels
{
    private static ClipboardProvider _clipboardProvider = new();
    private static readonly Logger _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

    private static readonly InMemoryAuthorizationService _inMemoryAuthorizationService = new();
    private static readonly InMemoryUsersService _inMemoryUsersService = new(_inMemoryAuthorizationService);
    private static readonly EmptyDialogService _emptyDialogService = new();
    private static readonly ReportErrorService _reportErrorService = new(_emptyDialogService, _clipboardProvider);
    private static readonly LoggedInUserService _loggedInUserService = new(
        _inMemoryAuthorizationService,
        _inMemoryUsersService,
        _reportErrorService);
    private static readonly EmptyLoadingPopupService _loadingPopupService = new();

    private static void ReloadViewModel(INavigationAware viewModel, Action? continueWith = null)
    {
        Task.Run(async () =>
        {
            await viewModel.OnNavigatedFrom();
            await viewModel.OnNavigatedTo();
            continueWith?.Invoke();
        });
    }

    private static UsersViewModel _usersViewModel = new(
        _inMemoryUsersService,
        _logger,
        _loadingPopupService,
        _emptyDialogService,
        _inMemoryAuthorizationService,
        _clipboardProvider,
        _reportErrorService,
        new EmptyUserProvidedImageService(),
        _loggedInUserService);
    public static UsersViewModel UsersViewModel
    {
        get
        {
            ReloadViewModel(
                _usersViewModel,
                () => _usersViewModel.SelectedUser = _usersViewModel.Users.FirstOrDefault());
            return _usersViewModel;
        }
    }


    private static LicensesViewModel _licensesViewModel = new(
        new InMemoryLicenseTypeService(),
        _logger,
        _loadingPopupService,
        _emptyDialogService,
        _inMemoryAuthorizationService,
        _clipboardProvider,
        _reportErrorService);
    public static LicensesViewModel LicensesViewModel
    {
        get
        {
            ReloadViewModel(
                _licensesViewModel,
                () => _licensesViewModel.SelectedLicense = _licensesViewModel.Licenses.FirstOrDefault());
            return _licensesViewModel;
        }
    }

    private static VehiclesViewModel _vehiclesViewModel = new(
        new InMemoryVehicleService(),
        new InMemoryLicenseTypeService(),
        _logger,
        _loadingPopupService,
        _emptyDialogService,
        _inMemoryAuthorizationService,
        _clipboardProvider,
        _reportErrorService,
        new EmptyUserProvidedImageService());

    public static VehiclesViewModel VehiclesViewModel
    {
        get
        {
            ReloadViewModel(
                _vehiclesViewModel,
                () => _vehiclesViewModel.SelectedVehicle = _vehiclesViewModel.Vehicles.FirstOrDefault());

            return _vehiclesViewModel;
        }
    }

    private static DriversViewModel _driversViewModel = new(
        new InMemoryDriverService(),
        new InMemoryDriversLicenseService(),
        new InMemoryLicenseTypeService(),
        _logger,
        _loadingPopupService,
        _emptyDialogService,
        _inMemoryAuthorizationService,
        _clipboardProvider,
        _reportErrorService,
        new EmptyUserProvidedImageService());

    public static DriversViewModel DriversViewModel
    {
        get
        {
            ReloadViewModel(
                _driversViewModel,
                () => _driversViewModel.SelectedDriver = _driversViewModel.Drivers.FirstOrDefault());

            return _driversViewModel;
        }
    }


}
