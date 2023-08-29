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

    private static UsersViewModel _usersViewModel = new(
        _inMemoryUsersService,
        _logger,
        new EmptyLoadingPopupService(),
        _emptyDialogService,
        _inMemoryAuthorizationService,
        _clipboardProvider,
        new ReportErrorService(_emptyDialogService, _clipboardProvider),
        new EmptyUserProvidedImageService());
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
