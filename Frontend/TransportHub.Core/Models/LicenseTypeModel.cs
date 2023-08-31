using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Models;

public partial class LicenseTypeModel : ObservableObject
{
    [ObservableProperty]
    private string? _id;
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string? _description;
    [ObservableProperty]
    private int _minimalAgeOfHolder;
    [ObservableProperty]
    private int? _alternativeMinimalAgeOfHolder;
    [ObservableProperty]
    private string? _conditionForAlternativeMinimalAge;
    [ObservableProperty]
    private bool _disabled;
    [ObservableProperty]
    private bool _isDirty;
}
