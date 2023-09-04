using CommunityToolkit.Mvvm.ComponentModel;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Models;

public partial class DriverModel : ObservableObject
{
    [ObservableProperty]
    private string _id;
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _picture;
    [ObservableProperty]
    private string _nationality;
    [ObservableProperty]
    private string _baseLocation;
    [ObservableProperty]
    private bool _disabled;
    [ObservableProperty]
    private bool _isDirty;

    public DriverModel() {}

    [MapperConstructor]
    public DriverModel(string id, string name, string picture, string nationality, string baseLocation, bool disabled)
    {
        Id = id;
        Name = name;
        Picture = picture;
        Nationality = nationality;
        BaseLocation = baseLocation;
        Disabled = disabled;
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

    public DriverModel Clone()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Picture = Picture,
            Nationality = Nationality,
            BaseLocation = BaseLocation,
            Disabled = Disabled,
            IsDirty = false,
        };
    }
}
