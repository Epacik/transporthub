using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;

namespace TransportHub.Core.Models;

public partial class UserModel : ObservableObject
{
    [ObservableProperty]
    private string? _id;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _picture;

    [ObservableProperty]
    private DateTime? _passwordExpirationDate;

    [ObservableProperty]
    private UserType _userType;

    [ObservableProperty]
    private bool _multiLogin;

    [ObservableProperty]
    private bool _disabled;

    [ObservableProperty]
    private bool _isDirty;

    [RelayCommand]
    private void ResetPasswordExpirationDate()
    {
        PasswordExpirationDate = null;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName != nameof(IsDirty))
        {
            IsDirty = true;
        }
    }

    public UserModel() { }

    [MapperConstructor]
    public UserModel(
        string? id,
        string? name,
        string? picture,
        DateTime? passwordExpirationDate,
        UserType userType,
        bool multiLogin,
        bool disabled)
    {
        Id = id;
        Name = name;
        Picture = picture;
        PasswordExpirationDate = passwordExpirationDate;
        UserType = userType;
        MultiLogin = multiLogin;
        Disabled = disabled;
    }
}
