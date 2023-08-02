using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using Frontend.Extensions;
using Frontend.Helpers;
using Frontend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModels;

internal partial class StartupSettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly IDialogService _dialogService;
    [ObservableProperty]
    private string? _serverAddress;

    [ObservableProperty]
    private bool _demoMode;
    private bool _demoModeOriginal;

    public StartupSettingsViewModel(
        ISettingsService settings,
        IDialogService dialogService)
    {
        _settings = settings;
        _dialogService = dialogService;
        ServerAddress = _settings.Read(Settings.IpAddress);
        DemoMode = _settings.ReadBool(Settings.DemoMode);
        _demoModeOriginal = DemoMode;
    }

    public async Task SaveSettings()
    {
        _settings.Write(Settings.IpAddress, ServerAddress);
        _settings.Write(Settings.DemoMode, DemoMode);

        if (DemoMode != _demoModeOriginal)
        {
            await _dialogService.ShowAlertAsync(
                "Tryb demonstracyjny",
                $"""
                Tryb demonstracyjny został {(DemoMode ? "włączony" : "wyłączony")}.
                Należy ponownie uruchomić transporthub
                """);
            Environment.Exit(0);
        }
    }

    public async Task ResetSettings()
    {
        var result = await _dialogService.ShowConfirmation(
            "Reset ustawień",
            "Czy na pewno chcesz zresetować ustawienia?");

        if (!result)
            return;

        _settings.Write(Settings.IpAddress, Settings.IpAddress.DefaultValue<string>());
        _settings.Write(Settings.DemoMode, Settings.DemoMode.DefaultValue<bool>());
    }
}
