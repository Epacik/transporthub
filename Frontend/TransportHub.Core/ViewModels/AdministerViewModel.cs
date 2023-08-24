using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Common;

namespace TransportHub.Core.ViewModels;

public partial class AdministerViewModel : ObservableObject
{
    public AdministerViewModel(
        UsersViewModel usersViewModel,
        DriversViewModel driversViewModel,
        VehiclesViewModel vehiclesViewModel)
    {
        _usersViewModel = usersViewModel;
        _driversViewModel = driversViewModel;
        _vehiclesViewModel = vehiclesViewModel;
        _routeMap = new Dictionary<string, INavigationAwareViewModel>()
        {
            [Routes.Users] = usersViewModel,
            [Routes.Drivers] = driversViewModel,
            [Routes.Vehicles] = vehiclesViewModel,
        };
    }

    private readonly Dictionary<string, INavigationAwareViewModel> _routeMap;

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
}
