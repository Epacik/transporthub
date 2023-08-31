using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Models;

public partial class DriversLicenseModel : ObservableObject
{
    [ObservableProperty]
    private string _id;
    [ObservableProperty]
    private int _driver;
    [ObservableProperty]
    private int _license;
    [ObservableProperty]
    private bool _disabled;
    [ObservableProperty]
    private bool _isDirty;
}
