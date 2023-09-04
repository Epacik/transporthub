using CommunityToolkit.Mvvm.ComponentModel;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    private string _requiredLicense;
    [ObservableProperty]
    private string? _registrationNumber;
    [ObservableProperty]
    private string? _vin;
    [ObservableProperty]
    private bool _disabled;
    [ObservableProperty]
    private bool _isDirty;
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName != nameof(IsDirty))
        {
            IsDirty = true;
        }
    }

    public VehicleModel() {}

    [MapperConstructor]
    public VehicleModel(string id, string name, int vehicleType, string? picture, string requiredLicense, string? registrationNumber, string? vin, bool disabled)
    {
        Id = id;
        Name = name;
        VehicleType = vehicleType;
        Picture = picture;
        RequiredLicense = requiredLicense;
        RegistrationNumber = registrationNumber;
        Vin = vin;
        Disabled = disabled;
    }

    public VehicleModel Clone()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            VehicleType = VehicleType,
            Picture = Picture,
            RequiredLicense = RequiredLicense,
            RegistrationNumber = RegistrationNumber,
            Vin = Vin,
            Disabled = Disabled,
            IsDirty = false,
        };
    }
}
