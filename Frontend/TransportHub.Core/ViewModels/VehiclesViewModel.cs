using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;
using TransportHub.Core.Mappers;
using TransportHub.Core.Models;
using TransportHub.Core.Services;
using TransportHub.Services;

namespace TransportHub.Core.ViewModels;

public partial class VehiclesViewModel : ObservableObject, INavigationAware
{
    private readonly IVehicleService _vehicleService;
    private readonly ILicenseTypeService _licenseTypeService;
    private readonly ILogger _logger;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IDialogService _dialogService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IClipboardProvider _clipboardProvider;
    private readonly IReportErrorService _reportErrorService;
    private readonly IUserProvidedImageService _userProvidedImageService;

    public VehiclesViewModel(
        IVehicleService vehicleTypeService,
        ILicenseTypeService licenseTypeService,
        ILogger logger,
        ILoadingPopupService loadingPopupService,
        IDialogService dialogService,
        IAuthorizationService authorizationService,
        IClipboardProvider clipboardProvider,
        IReportErrorService reportErrorService,
        IUserProvidedImageService userProvidedImageService)
    {
        _vehicleService = vehicleTypeService;
        _licenseTypeService = licenseTypeService;
        _logger = logger;
        _loadingPopupService = loadingPopupService;
        _dialogService = dialogService;
        _authorizationService = authorizationService;
        _clipboardProvider = clipboardProvider;
        _reportErrorService = reportErrorService;
        _userProvidedImageService = userProvidedImageService;

        VehicleTypes.Add("Bus");
        VehicleTypes.Add("Ciężarówka");
        VehicleTypes.Add("Przyczepka");
        VehicleTypes.Add("Przyczepa");
        VehicleTypes.Add("Ciągnik siodłowy");
        VehicleTypes.Add("Naczepa");
    }

    [ObservableProperty]
    private ObservableCollection<VehicleModel> _vehicles = new();

    [ObservableProperty]
    private ObservableCollection<LicenseTypeModel> _licenses = new();

    [ObservableProperty]
    private ObservableCollection<string> _licenseIds = new();

    [ObservableProperty]
    private bool _addingVehicle;

    [ObservableProperty]
    private VehicleModel? _editedVehicle;

    [ObservableProperty]
    private VehicleModel? _selectedVehicle;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<string> _vehicleTypes = new();

    partial void OnSelectedVehicleChanging(VehicleModel? oldValue, VehicleModel? newValue)
    {
        if (newValue is null)
        {
            EditedVehicle = null;
            return;
        }

        EditedVehicle = newValue.Clone();
        AddingVehicle = false;
    }


    [RelayCommand]
    private async Task CloseVehicle()
    {
        if (EditedVehicle is null ||
            (EditedVehicle.IsDirty && !(await ConfirmUnsavedChanges())))
        {
            return;
        }

        SelectedVehicle = null;
        EditedVehicle = null;
        await OnNavigatedFrom();
        await OnNavigatedTo();
    }

    [RelayCommand]
    private async Task RevertChanges()
    {
        if (EditedVehicle is null || !EditedVehicle.IsDirty)
            return;

        var result = await _dialogService.ShowConfirmation("Cofnij zmiany", "Czy na pewno chcesz cofnąć zmiany?");

        if (result)
            EditedVehicle = SelectedVehicle?.Clone();
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");
        var result = await _vehicleService.Update(EditedVehicle!.Id!, EditedVehicle!.ToVehicleAddDto());

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var clone = EditedVehicle!.Clone();
        var index = Vehicles.IndexOf(SelectedVehicle!);
        Vehicles[index] = clone;
        SelectedVehicle = clone;
        await _loadingPopupService.Hide();
    }

    [RelayCommand]
    private async Task RemoveVehicle()
    {
        var remove = await _dialogService.ShowConfirmation(
            "Usuń pojazd",
            """
            Czy na pewno chcesz usunąć pojazd?
            """);

        if (!remove)
            return;

        await _loadingPopupService.Show("Zapisywanie zmian");
        var result = await _vehicleService.Remove(EditedVehicle!.Id!);

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        await _loadingPopupService.Hide();

        SelectedVehicle = null;
        EditedVehicle = null;
        await OnNavigatedFrom();
        await OnNavigatedTo();
    }

    [RelayCommand]
    private void AddVehicle()
    {
        var newUser = new VehicleModel();
        Vehicles.Add(newUser);
        SelectedVehicle = newUser;
        AddingVehicle = true;
    }

    [RelayCommand]
    private async Task AddNewVehicle()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");
        var dto = EditedVehicle!.ToVehicleAddDto();
        var result = await _vehicleService.Add(dto);

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var clone = EditedVehicle!.Clone();
        var index = Vehicles.IndexOf(SelectedVehicle!);
        Vehicles[index] = clone;
        SelectedVehicle = clone;

        await _loadingPopupService.Hide();

        await _dialogService.ShowAlertAsync(
            "Zapisano",
            """
                    Dodano nowy pojazd
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

        EditedVehicle!.Picture = result.Unwrap();
    }

    private Task<bool> ConfirmUnsavedChanges()
    {
        return _dialogService.ShowConfirmation("Niezapisane zmiany", "Istnieją niezapisane zmiany.\nCzy kontynuować?");
    }

    public Task OnNavigatedFrom()
    {
        Vehicles.Clear();
        Licenses.Clear();
        LicenseIds.Clear();
        SelectedVehicle = null;
        return Task.CompletedTask;
    }

    public async Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Loading Vehicles");

        IsLoading = true;


        var licenseResult = await _licenseTypeService.List();

        if (licenseResult.IsError)
        {
            var ex = licenseResult.UnwrapErr();
            _logger.Error(ex, "Error while loading Vehicles");
            await _loadingPopupService.Hide();
            await _dialogService.ShowAlertAsync("Błąd wczytywania praw jazdy", ex.ToString());
            return;
        }

        var licenses = licenseResult.Unwrap().ToLicenseTypeModels();

        foreach (var license in licenses.OrderBy(x => x.Name))
        {
            Licenses.Add(license);
            LicenseIds.Add(license.Id!);
        }

        var result = await _vehicleService.List();

        if (result.IsError)
        {
            var ex = result.UnwrapErr();
            _logger.Error(ex, "Error while loading Vehicles");
            await _loadingPopupService.Hide();
            await _dialogService.ShowAlertAsync("Błąd", ex.ToString());
            return;
        }

        var vehicles = result.Unwrap().ToVehicleModels();

        foreach (var user in vehicles.OrderBy(x => x.Name))
        {
            Vehicles.Add(user);
        }

        OnPropertyChanged(nameof(Vehicles));
        OnPropertyChanged(nameof(Licenses));

        IsLoading = false;
    }
}
