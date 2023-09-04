using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Discord;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;
using TransportHub.Api.Dtos;
using TransportHub.Core.Mappers;
using TransportHub.Core.Models;
using TransportHub.Core.Services;
using TransportHub.Services;

namespace TransportHub.Core.ViewModels;

internal partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly IUsersService _usersService;
    private readonly ILogger _logger;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IDialogService _dialogService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IReportErrorService _reportErrorService;
    private readonly IUserProvidedImageService _userProvidedImageService;

    public SettingsViewModel(
        IUsersService usersService,
        ILogger logger,
        ILoadingPopupService loadingPopupService,
        IDialogService dialogService,
        IAuthorizationService authorizationService,
        IReportErrorService reportErrorService,
        IUserProvidedImageService userProvidedImageService,
        ILoggedInUserService loggedInUserService,
        UsedLicensesViewModel usedLicensesViewModel)
    {
        _usersService = usersService;
        _logger = logger;
        _loadingPopupService = loadingPopupService;
        _dialogService = dialogService;
        _authorizationService = authorizationService;
        _reportErrorService = reportErrorService;
        _userProvidedImageService = userProvidedImageService;
        LoggedInUserService = loggedInUserService;
        _usedLicensesViewModel = usedLicensesViewModel;
    }

    private UserDto _user;

    [ObservableProperty]
    private UserModel? _editedUser;

    [ObservableProperty]
    private bool _changePasswordEnabled;

    [ObservableProperty]
    private string? _passwordValidationError;

    [ObservableProperty]
    private bool _revealPassword;

    [ObservableProperty]
    private string? _newPassword;

    [ObservableProperty]
    private UsedLicensesViewModel _usedLicensesViewModel;

    [ObservableProperty]
    private bool _isLicensesTabOpen;

    partial void OnIsLicensesTabOpenChanged(bool value)
    {
        _ = Task.Run(async () =>
        {
            if (value)
            {
                await UsedLicensesViewModel.OnNavigatedTo();
            }
            else
            {
                await UsedLicensesViewModel.OnNavigatedFrom();
            }
        });
    }

    partial void OnNewPasswordChanged(string? oldValue, string? newValue)
    {
        if (oldValue == newValue)
            return;

        char[] special = { '!', '@', '#', '$', '%', '^', '&', '*', ':', ';', ',', '.', '<', '>', '_', '-', };
        bool isValidChar(char x)
            => (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z') || (x >= '0' && x <= '9') || special.Contains(x);

        if (newValue is not null && !newValue.All(x => isValidChar(x)))
            NewPassword = oldValue;

        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(IsPasswordValid));
    }

    public bool IsPasswordValid => ChangePasswordEnabled && ValidatePassword(NewPassword);

    public bool IsValid
    {
        get
        {
            if (ChangePasswordEnabled && !ValidatePassword(NewPassword))
                return false;

            return EditedUser?.IsDirty == true || ChangePasswordEnabled;
        }
    }

    public ILoggedInUserService LoggedInUserService { get; }

    private bool ValidatePassword(string? password)
    {
        List<string> errors = new();

        password ??= "";

        if (!password.Any(x => x >= 'a' && x <= 'z'))
            errors.Add("Hasło musi zawierać co najmniej jedną małą literę");

        if (!password.Any(x => x >= 'A' && x <= 'Z'))
            errors.Add("Hasło musi zawierać co najmniej jedną wielką literę");

        if (!password.Any(x => x >= '0' && x <= '9'))
            errors.Add("Hasło musi zawierać co najmniej jedną cyfrę");

        char[] special = { '!', '@', '#', '$', '%', '^', '&', '*', ':', ';', ',', '.', '<', '>', '_', '-', };

        if (!password.Any(x => special.Contains(x)))
            errors.Add($"Hasło musi zawierać co najmniej jeden z podanych znaków {string.Join("", special)}");

        if (password.Length < 8)
            errors.Add("Hasło musi mieć co najmniej 8 znaków");

        PasswordValidationError = string.Join("\n", errors);

        return errors.Count == 0;
    }

    public Task OnNavigatedFrom()
    {
        return Task.CompletedTask;
    }

    public async Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        await _loadingPopupService.Show("Ładowanie danych użytkownika");
        var result = await _usersService.GetUser(_authorizationService.UserData!.User!);

        if (result.IsError)
        {
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        EditedUser = result.Unwrap().ToUserModel();
        EditedUser.PropertyChanged += EditedUser_PropertyChanged;
        await _loadingPopupService.Hide();
    }

    private void EditedUser_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(IsPasswordValid));
    }

    [RelayCommand]
    private async Task ChangePicture()
    {
        var result = await _userProvidedImageService.GetImage();
        if (result.IsError)
        {
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        EditedUser!.Picture = result.Unwrap();
    }

    [RelayCommand]
    private async Task RevertChanges()
    {
        if (EditedUser is null || !EditedUser.IsDirty)
            return;

        var result = await _dialogService.ShowConfirmation("Cofnij zmiany", "Czy na pewno chcesz cofnąć zmiany?");

        if (result)
        {
            EditedUser!.PropertyChanged -= EditedUser_PropertyChanged;
            EditedUser = _user.ToUserModel();
            EditedUser.PropertyChanged += EditedUser_PropertyChanged;
            ChangePasswordEnabled = false;
            NewPassword = null;
            PasswordValidationError = null;
        }
    }

    [RelayCommand]
    private async Task SaveChanges()
    {

        if (EditedUser?.IsDirty ?? false)
        {
            await _loadingPopupService.Show("Zapisywanie zmian");
            var result = await _usersService.Update(EditedUser!.Id!, EditedUser!.ToUserUpdateDto());

            await _loadingPopupService.Hide();
            if (result.IsError)
            {
                await _reportErrorService.ShowError(result.UnwrapErr());
                return;
            }
        }

        if (ChangePasswordEnabled)
        {
            await _loadingPopupService.Show("Zmiana hasła");
            var result = await _usersService.UpdatePassword(EditedUser!.Id!, new(NewPassword!));

            ChangePasswordEnabled = false;
            NewPassword = null;
            PasswordValidationError = null;
            await _loadingPopupService.Hide();
            if (result.IsError)
            {
                await _reportErrorService.ShowError(result.UnwrapErr());
                return;
            }
            
        }


        await LoggedInUserService.ForceRefreshAsync();
    }

    [RelayCommand]
    private void ChangePassword()
    {
        ChangePasswordEnabled = true;
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(IsPasswordValid));
    }

}
