using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;
using TransportHub.Api.Dtos;
using TransportHub.Core.Services;
using TransportHub.Services;

namespace TransportHub.Core.ViewModels;

public partial class UsersViewModel : ObservableObject, INavigationAwareViewModel
{
    private readonly IUsersService _usersService;
    private readonly ILogger _logger;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IDialogService _dialogService;

    public UsersViewModel(
        IUsersService usersService,
        ILogger logger,
        ILoadingPopupService loadingPopupService,
        IDialogService dialogService)
    {
        _usersService = usersService;
        _logger = logger;
        _loadingPopupService = loadingPopupService;
        _dialogService = dialogService;
    }

    [ObservableProperty]
    private ObservableCollection<UserDto> _users = new();

    public Task OnNavigatedFrom()
    {
        Users.Clear();
        return Task.CompletedTask;
    }

    public async Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Loading users");

        await _loadingPopupService.Show("Ładowanie listy użytkowników");

        var result = await _usersService.ListUsers();

        if (result.IsError)
        {
            var ex = result.UnwrapErr();
            _logger.Error(ex, "Error while loading users");
            await _loadingPopupService.Hide();
            await _dialogService.ShowAlertAsync("Błąd", ex.ToString());
            return;
        }

        var users = result.Unwrap();

        foreach (var user in users)
        {
            Users.Add(user);
        }

        OnPropertyChanged(nameof(Users));

        await _loadingPopupService.Hide();
    }
}
