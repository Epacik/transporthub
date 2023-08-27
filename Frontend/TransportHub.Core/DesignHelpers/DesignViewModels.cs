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

namespace TransportHub.Core.DesignHelpers;

public static class DesignViewModels
{

    private static readonly Logger _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

    private static UsersViewModel _usersViewModel = new(
        new InMemoryUsersService(),
        _logger,
        new EmptyLoadingPopupService(),
        new EmptyDialogService());
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
