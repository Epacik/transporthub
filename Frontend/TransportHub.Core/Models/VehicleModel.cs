using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Models;

public partial class VehicleModel : ObservableObject
{
    [ObservableProperty]
    private string _id;
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private int _vehicleType;
    [ObservableProperty]
    private string? _Picture;
    [ObservableProperty]
    private int _requiredLicense;
    [ObservableProperty]
    private string? _registrationNumber;
    [ObservableProperty]
    private string? _vin;
    [ObservableProperty]
    private bool _disabled;
    [ObservableProperty]
    private bool _isDirty;
}
