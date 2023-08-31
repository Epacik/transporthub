using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;
using TransportHub.Common;
using TransportHub.Core.Services;

namespace TransportHub.Core.ViewModels;

public partial class AdministerViewModel : ObservableObject, INavigationAware
{
    public AdministerViewModel(
        UsersViewModel usersViewModel,
        DriversViewModel driversViewModel,
        VehiclesViewModel vehiclesViewModel,
        ClientsViewModel clientsViewModel,
        LicensesViewModel licensesViewModel,
        ILoggedInUserService loggedInUserService)
    {
        _usersViewModel = usersViewModel;
        _driversViewModel = driversViewModel;
        _vehiclesViewModel = vehiclesViewModel;
        _clientsViewModel = clientsViewModel;
        _licensesViewModel = licensesViewModel;
        LoggedInUserService = loggedInUserService;

        _routeMap = new Dictionary<string, INavigationAware>()
        {
            [Routes.Users] = usersViewModel,
            [Routes.Drivers] = driversViewModel,
            [Routes.Vehicles] = vehiclesViewModel,
            [Routes.Clients] = clientsViewModel,
            [Routes.Licenses] = licensesViewModel,
        };
    }

    private readonly Dictionary<string, INavigationAware> _routeMap;

    [ObservableProperty]
    private string? _selectedTab;
    partial void OnSelectedTabChanged(string? oldValue, string? newValue)
    {
        Task.Run(async () =>
        {
            if (!string.IsNullOrEmpty(oldValue))
                await (_routeMap[oldValue]?.OnNavigatedFrom() ?? Task.CompletedTask);

            if (!string.IsNullOrEmpty(newValue))
                await (_routeMap[newValue]?.OnNavigatedTo() ?? Task.CompletedTask);
        });
    }

    [ObservableProperty]
    private UsersViewModel _usersViewModel;
    [ObservableProperty]
    private DriversViewModel _driversViewModel;
    [ObservableProperty]
    private VehiclesViewModel _vehiclesViewModel;
    [ObservableProperty]
    private ClientsViewModel _clientsViewModel;
    [ObservableProperty]
    private LicensesViewModel _licensesViewModel;

    public ILoggedInUserService LoggedInUserService { get; }

    public Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        throw new NotImplementedException();
    }

    public Task OnNavigatedFrom()
    {
        return Task.CompletedTask;
    }
}
