using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransportHub.Core.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly Random _random = new();

    [ObservableProperty]
    private int _deliveriesEnRoute;

    [ObservableProperty]
    private int _newIncidents;

    [ObservableProperty]
    private int _newOrders;

    [ObservableProperty]
    private int _completedOrders;

    public DashboardViewModel()
    {
        DeliveriesEnRoute = _random.Next(3, 15);
        NewIncidents = _random.Next(3, 15);
        NewOrders = _random.Next(3, 15);
        CompletedOrders = _random.Next(3, 15);
    }
}
