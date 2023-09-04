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

public partial class LicensesViewModel : ObservableObject, INavigationAware
{
    private readonly ILicenseTypeService _licenseTypeService;
    private readonly ILogger _logger;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IDialogService _dialogService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IClipboardProvider _clipboardProvider;
    private readonly IReportErrorService _reportErrorService;

    public LicensesViewModel(
        ILicenseTypeService licenseTypeService,
        ILogger logger,
        ILoadingPopupService loadingPopupService,
        IDialogService dialogService,
        IAuthorizationService authorizationService,
        IClipboardProvider clipboardProvider,
        IReportErrorService reportErrorService)
    {
        _licenseTypeService = licenseTypeService;
        _logger = logger;
        _loadingPopupService = loadingPopupService;
        _dialogService = dialogService;
        _authorizationService = authorizationService;
        _clipboardProvider = clipboardProvider;
        _reportErrorService = reportErrorService;
    }

    [ObservableProperty]
    private ObservableCollection<LicenseTypeModel> _licenses = new();

    [ObservableProperty]
    private bool _addingLicense;

    [ObservableProperty]
    private LicenseTypeModel? _editedLicense;

    [ObservableProperty]
    private LicenseTypeModel? _selectedLicense;

    [ObservableProperty]
    private bool _isLoading;

    partial void OnSelectedLicenseChanging(LicenseTypeModel? oldValue, LicenseTypeModel? newValue)
    {
        if (newValue is null)
        {
            EditedLicense = null;
            return;
        }

        EditedLicense = newValue.Clone();
        AddingLicense = false;
    }


    [RelayCommand]
    private async Task CloseLicense()
    {
        if (EditedLicense is null ||
            (EditedLicense.IsDirty && !(await ConfirmUnsavedChanges())))
        {
            return;
        }

        SelectedLicense = null;
        EditedLicense = null;
        await OnNavigatedFrom();
        await OnNavigatedTo();
    }

    [RelayCommand]
    private async Task RevertChanges()
    {
        if (EditedLicense is null || !EditedLicense.IsDirty)
            return;

        var result = await _dialogService.ShowConfirmation("Cofnij zmiany", "Czy na pewno chcesz cofnąć zmiany?");

        if (result)
            EditedLicense = SelectedLicense?.Clone();
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");
        var result = await _licenseTypeService.Update(EditedLicense!.Id!, EditedLicense!.ToLicenseTypeAddDto());

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var clone = EditedLicense!.Clone();
        var index = Licenses.IndexOf(SelectedLicense!);
        Licenses[index] = clone;
        SelectedLicense = clone;
        await _loadingPopupService.Hide();
    }

    [RelayCommand]
    private async Task RemoveLicense()
    {
        var remove = await _dialogService.ShowConfirmation(
            "Usuń prawo jazdy",
            """
            Czy na pewno chcesz usunąć prawo jazdy?
            """);

        if (!remove)
            return;

        await _loadingPopupService.Show("Zapisywanie zmian");
        var result = await _licenseTypeService.Remove(EditedLicense!.Id!);

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        await _loadingPopupService.Hide();

        SelectedLicense = null;
        EditedLicense = null;
        await OnNavigatedFrom();
        await OnNavigatedTo();
    }

    [RelayCommand]
    private void AddLicense()
    {
        var newUser = new LicenseTypeModel();
        Licenses.Add(newUser);
        SelectedLicense = newUser;
        AddingLicense = true;
    }

    [RelayCommand]
    private async Task AddNewLicense()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");
        var dto = EditedLicense!.ToLicenseTypeAddDto();
        var result = await _licenseTypeService.Add(dto);

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var clone = EditedLicense!.Clone();
        var index = Licenses.IndexOf(SelectedLicense!);
        Licenses[index] = clone;
        SelectedLicense = clone;

        await _loadingPopupService.Hide();

        await _dialogService.ShowAlertAsync(
            "Zapisano",
            """
                    Dodano nowe prawo jazdy
                    """);
    }

    [RelayCommand]
    private void RemoveAlternativeAge()
    {
        if (EditedLicense is null)
            return;

        EditedLicense.AlternativeMinimalAgeOfHolder = null;
        EditedLicense.ConditionForAlternativeMinimalAge = null;
    }

    private Task<bool> ConfirmUnsavedChanges()
    {
        return _dialogService.ShowConfirmation("Niezapisane zmiany", "Istnieją niezapisane zmiany.\nCzy kontynuować?");
    }

    public Task OnNavigatedFrom()
    {
        Licenses.Clear();
        SelectedLicense = null;
        return Task.CompletedTask;
    }

    public async Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Loading Licenses");

        IsLoading = true;

        var result = await _licenseTypeService.List();

        if (result.IsError)
        {
            var ex = result.UnwrapErr();
            _logger.Error(ex, "Error while loading Licenses");
            await _loadingPopupService.Hide();
            await _dialogService.ShowAlertAsync("Błąd", ex.ToString());
            return;
        }

        var licenses = result.Unwrap().ToLicenseTypeModels();

        foreach (var user in licenses.OrderBy(x => x.Name))
        {
            Licenses.Add(user);
        }

        OnPropertyChanged(nameof(Licenses));

        IsLoading = false;
    }
}
