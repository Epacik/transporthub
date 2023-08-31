using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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
}
