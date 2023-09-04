using CommunityToolkit.Mvvm.ComponentModel;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName != nameof(IsDirty))
        {
            IsDirty = true;
        }
    }

    public override string ToString()
    {
        return Name;
    }

    public LicenseTypeModel() {}

    [MapperConstructor]
    public LicenseTypeModel(string? id, string? name, string? description, int minimalAgeOfHolder, int? alternativeMinimalAgeOfHolder, string? conditionForAlternativeMinimalAge, bool disabled)
    {
        Id = id;
        Name = name;
        Description = description;
        MinimalAgeOfHolder = minimalAgeOfHolder;
        AlternativeMinimalAgeOfHolder = alternativeMinimalAgeOfHolder;
        ConditionForAlternativeMinimalAge = conditionForAlternativeMinimalAge;
        Disabled = disabled;
    }

    internal LicenseTypeModel Clone()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Description = Description,
            MinimalAgeOfHolder = MinimalAgeOfHolder,
            AlternativeMinimalAgeOfHolder = AlternativeMinimalAgeOfHolder,
            ConditionForAlternativeMinimalAge = ConditionForAlternativeMinimalAge,
            Disabled = Disabled,
            IsDirty = false,
        };
    }
}
