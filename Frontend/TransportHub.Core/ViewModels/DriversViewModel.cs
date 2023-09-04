using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;
using TransportHub.Core.Mappers;
using TransportHub.Core.Models;
using TransportHub.Core.Services;
using TransportHub.Services;

namespace TransportHub.Core.ViewModels;

public partial class DriversViewModel : ObservableObject, INavigationAware
{
    private readonly IDriverService _driverService;
    private readonly IDriversLicenseService _driversLicenseService;
    private readonly ILicenseTypeService _licenseTypeService;
    private readonly ILogger _logger;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IDialogService _dialogService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IClipboardProvider _clipboardProvider;
    private readonly IReportErrorService _reportErrorService;
    private readonly IUserProvidedImageService _userProvidedImageService;

    public DriversViewModel(
        IDriverService driverTypeService,
        IDriversLicenseService driversLicenseService,
        ILicenseTypeService licenseTypeService,
        ILogger logger,
        ILoadingPopupService loadingPopupService,
        IDialogService dialogService,
        IAuthorizationService authorizationService,
        IClipboardProvider clipboardProvider,
        IReportErrorService reportErrorService,
        IUserProvidedImageService userProvidedImageService)
    {
        _driverService = driverTypeService;
        _driversLicenseService = driversLicenseService;
        _licenseTypeService = licenseTypeService;
        _logger = logger;
        _loadingPopupService = loadingPopupService;
        _dialogService = dialogService;
        _authorizationService = authorizationService;
        _clipboardProvider = clipboardProvider;
        _reportErrorService = reportErrorService;
        _userProvidedImageService = userProvidedImageService;

        SelectedLicenses.CollectionChanged += SelectedLicenses_CollectionChanged;
    }

    [ObservableProperty]
    private ObservableCollection<DriverModel> _drivers = new();

    [ObservableProperty]
    private ObservableCollection<LicenseTypeModel> _licenses = new();

    [ObservableProperty]
    private ObservableCollection<LicenseTypeModel> _selectedLicenses = new();

    [ObservableProperty]
    private ObservableCollection<DriversLicenseModel> _driversLicenses = new();

    [ObservableProperty]
    private bool _addingDriver;

    [ObservableProperty]
    private DriverModel? _editedDriver;

    [ObservableProperty]
    private DriverModel? _selectedDriver;

    [ObservableProperty]
    private bool _isLoading;

    partial void OnSelectedDriverChanging(DriverModel? oldValue, DriverModel? newValue)
    {
        SelectedLicenses.Clear();
        if (newValue is null)
        {
            EditedDriver = null;
            return;
        }

        var driversLicenses = DriversLicenses.Where(x => x.Driver == newValue.Id);

        var licenses = Licenses.Where(
            x => driversLicenses.Any(dl => dl.License == x.Id));

        foreach(var license in licenses)
        {
            SelectedLicenses.Add(license);
        }

        EditedDriver = newValue.Clone();
        AddingDriver = false;
    }

    private void SelectedLicenses_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (EditedDriver is not null)
            EditedDriver.IsDirty = true;
    }

    [RelayCommand]
    private async Task CloseDriver()
    {
        if (EditedDriver is null ||
            (EditedDriver.IsDirty && !(await ConfirmUnsavedChanges())))
        {
            return;
        }

        SelectedDriver = null;
        EditedDriver = null;
        await OnNavigatedFrom();
        await OnNavigatedTo();
    }

    [RelayCommand]
    private async Task RevertChanges()
    {
        if (EditedDriver is null || !EditedDriver.IsDirty)
            return;

        var result = await _dialogService.ShowConfirmation("Cofnij zmiany", "Czy na pewno chcesz cofnąć zmiany?");

        if (result)
            EditedDriver = SelectedDriver?.Clone();
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");
        var result = await _driverService.Update(EditedDriver!.Id!, EditedDriver!.ToDriverAddDto());

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var driversLicenses = DriversLicenses.Where(x => x.Driver == EditedDriver.Id);

        var selectedLicenses = SelectedLicenses;

        var licensesToAdd = selectedLicenses.Where(
            x => !driversLicenses.Any(l => l.License == x.Id));

        var licensesToRemove = driversLicenses.Where(
            x => !selectedLicenses.Any(l => l.Id == x.License));

        List<Exception> errors = new();

        foreach (var license in licensesToAdd)
        {
            var res = await _driversLicenseService.Add(new()
            {
                Driver = EditedDriver.Id,
                License = license.Id!,
                Disabled = false,
            });

            if (res.IsError)
            {
                var ex = res.UnwrapErr();
                _logger.Error(ex, "Error while saving Driver");
                await _loadingPopupService.Hide();

                errors.Add(ex);
            }
        }

        foreach (var license in licensesToRemove)
        {
            var res = await _driversLicenseService.Remove(license.Id!);

            if (res.IsError)
            {
                var ex = res.UnwrapErr();
                _logger.Error(ex, "Error while saving Driver");
                await _loadingPopupService.Hide();

                errors.Add(ex);
            }
        }

        var driversLicenseResult = await _driversLicenseService.List();

        if (driversLicenseResult.IsError)
        {
            var ex = driversLicenseResult.UnwrapErr();
            _logger.Error(ex, "Error while loading Drivers");
            await _loadingPopupService.Hide();
            await _dialogService.ShowAlertAsync("Błąd wczytywania praw jazdy kierowców", ex.ToString());
            return;
        }

        DriversLicenses.Clear();
        var driversLicensesRef = driversLicenseResult.Unwrap().ToDriversLicenseModels();

        foreach (var dl in driversLicensesRef)
        {
            DriversLicenses.Add(dl);
        }

        var clone = EditedDriver!.Clone();
        var index = Drivers.IndexOf(SelectedDriver!);
        Drivers[index] = clone;
        SelectedDriver = clone;
        await _loadingPopupService.Hide();
    }

    [RelayCommand]
    private async Task RemoveDriver()
    {
        var remove = await _dialogService.ShowConfirmation(
            "Usuń kierowcę",
            """
            Czy na pewno chcesz usunąć kierowcę?
            """);

        if (!remove)
            return;

        await _loadingPopupService.Show("Zapisywanie zmian");
        var result = await _driverService.Remove(EditedDriver!.Id!);

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var licensesRes = await _driversLicenseService.List();

        if (licensesRes.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(licensesRes.UnwrapErr());
            return;
        }

        var licenses = licensesRes.Unwrap().Where(x => x.Driver == EditedDriver!.Id!);

        foreach (var license in licenses)
        {
            var res = await _driversLicenseService.Remove(license.Id!);

            if (res.IsError)
            {
                var ex = res.UnwrapErr();
                _logger.Error(ex, "Error while saving Driver");
                await _loadingPopupService.Hide();
            }
        }

        await _loadingPopupService.Hide();

        SelectedDriver = null;
        EditedDriver = null;
        await OnNavigatedFrom();
        await OnNavigatedTo();
    }

    [RelayCommand]
    private void AddDriver()
    {
        var newUser = new DriverModel();
        Drivers.Add(newUser);
        SelectedDriver = newUser;
        AddingDriver = true;
    }

    [RelayCommand]
    private async Task AddNewDriver()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");
        var dto = EditedDriver!.ToDriverAddDto();
        var result = await _driverService.Add(dto);

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var clone = EditedDriver!.Clone();
        var index = Drivers.IndexOf(SelectedDriver!);
        Drivers[index] = clone;
        SelectedDriver = clone;

        await _loadingPopupService.Hide();

        await _dialogService.ShowAlertAsync(
            "Zapisano",
            """
                    Dodano nowego kierowcę
                    """);
    }

    [RelayCommand]
    private async Task ChangePicture()
    {
        var result = await _userProvidedImageService.GetImage();
        if (result.IsError)
        {
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        EditedDriver!.Picture = result.Unwrap() ?? "";
    }

    private Task<bool> ConfirmUnsavedChanges()
    {
        return _dialogService.ShowConfirmation("Niezapisane zmiany", "Istnieją niezapisane zmiany.\nCzy kontynuować?");
    }

    public Task OnNavigatedFrom()
    {
        Drivers.Clear();
        Licenses.Clear();
        SelectedLicenses.Clear();
        SelectedDriver = null;
        return Task.CompletedTask;
    }

    public async Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Loading Drivers");

        IsLoading = true;


        var licenseResult = await _licenseTypeService.List();

        if (licenseResult.IsError)
        {
            var ex = licenseResult.UnwrapErr();
            _logger.Error(ex, "Error while loading Drivers");
            await _loadingPopupService.Hide();
            await _dialogService.ShowAlertAsync("Błąd wczytywania praw jazdy", ex.ToString());
            return;
        }

        var licenses = licenseResult.Unwrap().ToLicenseTypeModels();

        foreach (var license in licenses.OrderBy(x => x.Name))
        {
            Licenses.Add(license);
        }

        var driversLicenseResult = await _driversLicenseService.List();

        if (driversLicenseResult.IsError)
        {
            var ex = driversLicenseResult.UnwrapErr();
            _logger.Error(ex, "Error while loading Drivers");
            await _loadingPopupService.Hide();
            await _dialogService.ShowAlertAsync("Błąd wczytywania praw jazdy kierowców", ex.ToString());
            return;
        }

        var driversLicenses = driversLicenseResult.Unwrap().ToDriversLicenseModels();

        foreach (var dl in driversLicenses)
        {
            DriversLicenses.Add(dl);
        }


        var result = await _driverService.List();

        if (result.IsError)
        {
            var ex = result.UnwrapErr();
            _logger.Error(ex, "Error while loading Drivers");
            await _loadingPopupService.Hide();
            await _dialogService.ShowAlertAsync("Błąd", ex.ToString());
            return;
        }

        var drivers = result.Unwrap().ToDriverModels();

        foreach (var user in drivers.OrderBy(x => x.Name))
        {
            Drivers.Add(user);
        }

        OnPropertyChanged(nameof(Drivers));
        OnPropertyChanged(nameof(Licenses));

        IsLoading = false;
    }
}
