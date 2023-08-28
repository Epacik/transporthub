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
using TransportHub.Core.Services.Impl.Empty;
using TransportHub.Core.Services.InMemory.API;
using TransportHub.Core.ViewModels;
using TransportHub.Services.InMemory.API;

namespace TransportHub.Core.DesignHelpers;

public static class DesignViewModels
{
    private static IClipboard? _clipboard
    {
        get
        {
            var lifetime = Application.Current?.ApplicationLifetime;
            if (lifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.MainWindow?.Clipboard;

            else if (lifetime is ISingleViewApplicationLifetime singleView)
                return TopLevel.GetTopLevel(singleView.MainView)?.Clipboard;

            return null;
        }
    }
    private static readonly Logger _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

    private static readonly InMemoryAuthorizationService _inMemoryAuthorizationService = new();
    private static readonly InMemoryUsersService _inMemoryUsersService = new(_inMemoryAuthorizationService);

    private static UsersViewModel _usersViewModel = new(
        _inMemoryUsersService,
        _logger,
        new EmptyLoadingPopupService(),
        new EmptyDialogService(),
        _inMemoryAuthorizationService,
        _clipboard);
    public static UsersViewModel UsersViewModel
    {
        get
        {
            Task.Run(async () =>
            {
                await _usersViewModel.OnNavigatedFrom();
                await _usersViewModel.OnNavigatedTo();
                _usersViewModel.SelectedUser = _usersViewModel.Users.FirstOrDefault();
            });
            return _usersViewModel;
        }
    }
}
