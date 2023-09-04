using CommunityToolkit.Mvvm.ComponentModel;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Models;

public partial class DriversLicenseModel : ObservableObject
{
    [ObservableProperty]
    private string _id;
    [ObservableProperty]
    private string _driver;
    [ObservableProperty]
    private string _license;
    [ObservableProperty]
    private bool _isDirty;

    public DriversLicenseModel() {}

    [MapperConstructor]
    public DriversLicenseModel(string id, string driver, string license)
    {
        Id = id;
        Driver = driver;
        License = license;
        IsDirty = false;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName != nameof(IsDirty))
        {
            IsDirty = true;
        }
    }

    public DriversLicenseModel Clone()
    {
        return new()
        {
            Id = Id,
            Driver = Driver,
            License = License,
            IsDirty = false,
        };
    }
}
