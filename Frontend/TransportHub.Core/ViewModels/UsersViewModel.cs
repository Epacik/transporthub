using Avalonia.Input.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

public partial class UsersViewModel : ObservableObject, INavigationAware
{
    private readonly IUsersService _usersService;
    private readonly ILogger _logger;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IDialogService _dialogService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IClipboardProvider _clipboardProvider;
    private readonly IReportErrorService _reportErrorService;
    private readonly IUserProvidedImageService _userProvidedImageService;

    public UsersViewModel(
        IUsersService usersService,
        ILogger logger,
        ILoadingPopupService loadingPopupService,
        IDialogService dialogService,
        IAuthorizationService authorizationService,
        IClipboardProvider clipboardProvider,
        IReportErrorService reportErrorService,
        IUserProvidedImageService userProvidedImageService,
        ILoggedInUserService loggedInUserService)
    {
        _usersService = usersService;
        _logger = logger;
        _loadingPopupService = loadingPopupService;
        _dialogService = dialogService;
        _authorizationService = authorizationService;
        _clipboardProvider = clipboardProvider;
        _reportErrorService = reportErrorService;
        _userProvidedImageService = userProvidedImageService;
        LoggedInUserService = loggedInUserService;
        UserTypes = new ObservableCollection<UserType>
        {
            UserType.User,
            UserType.Manager,
            UserType.Admin,
        };
    }

    [ObservableProperty]
    private ObservableCollection<UserModel> _users = new();

    [ObservableProperty]
    private bool _addingUser;

    [ObservableProperty]
    private bool _enableIsActive;

    [ObservableProperty]
    private UserModel? _editedUser;

    [ObservableProperty]
    private UserModel? _selectedUser;

    [ObservableProperty]
    private bool _isLoading;

    partial void OnSelectedUserChanging(UserModel? oldValue, UserModel? newValue)
    {
        if (newValue is null)
        {
            EditedUser = null;
            return;
        }

        EditedUser = newValue.Clone();
        EnableIsActive = newValue?.Id != _authorizationService.UserData?.User;
        AddingUser = false;
    }


    [ObservableProperty]
    private ObservableCollection<UserType> _userTypes;


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
    private async Task CloseUser()
    {
        if (EditedUser is null ||
            (EditedUser.IsDirty && !(await ConfirmUnsavedChanges())))
        {
            return;
        }
       
        SelectedUser = null;
        EditedUser = null;
        await OnNavigatedFrom();
        await OnNavigatedTo();
    }

    [RelayCommand]
    private async Task RevertChanges()
    {
        if (EditedUser is null || !EditedUser.IsDirty)
            return;

        var result = await _dialogService.ShowConfirmation("Cofnij zmiany", "Czy na pewno chcesz cofnąć zmiany?");

        if (result)
            EditedUser = SelectedUser?.Clone();
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");
        var result = await _usersService.UpdateAsAdmin(EditedUser!.Id!,EditedUser!.ToUserAdminUpdateDto());

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var clone = EditedUser!.Clone();
        var index = Users.IndexOf(SelectedUser!);
        Users[index] = clone;
        SelectedUser = clone;
        await _loadingPopupService.Hide();

        if (_authorizationService.UserData?.User == clone.Id)
        {
            await LoggedInUserService.ForceRefreshAsync();
        }

    }

    [RelayCommand]
    private async Task RemoveUser()
    {
        var remove = await _dialogService.ShowConfirmation(
            "Usuń użytkownika",
            """
            Czy na pewno chcesz usunąć użytkownika?
            """);

        if (!remove)
            return;

        await _loadingPopupService.Show("Zapisywanie zmian");
        var result = await _usersService.Remove(EditedUser!.Id!);

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        await _loadingPopupService.Hide();

        SelectedUser = null;
        EditedUser = null;
        await OnNavigatedFrom();
        await OnNavigatedTo();
    }

    [RelayCommand]
    private void AddUser()
    {
        var newUser = new UserModel(null, "", null, null, UserType.User, false, true);
        Users.Add(newUser);
        SelectedUser = newUser;
        AddingUser = true;
    }

    [RelayCommand]
    private async Task AddNewUser()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");
        var dto = EditedUser!.ToUserAddDto();
        dto.Password = RandomString(20);
        var result = await _usersService.Add(dto);

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var clone = EditedUser!.Clone();
        var index = Users.IndexOf(SelectedUser!);
        Users[index] = clone;
        SelectedUser = clone;

        await (_clipboardProvider.Get()?.SetTextAsync(dto.Password) ?? Task.CompletedTask);
        await _loadingPopupService.Hide();

        await _dialogService.ShowAlertAsync(
            "Zapisano",
            """
                    Dodano nowego użytkownika
                    Nowe hasło zostało zapisane w schowku.
                    """);
    }

    [RelayCommand]
    private async Task ResetPassword()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");

        var newPassword = RandomString(20);

        var result = await _usersService.UpdatePassword(EditedUser!.Id!, new(newPassword));
        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }
        await (_clipboardProvider.Get()?.SetTextAsync(newPassword) ?? Task.CompletedTask);

        await _loadingPopupService.Hide();

        await _dialogService.ShowAlertAsync(
            "Zresetowano",
            """
                    Hasło zostało zresetowane.
                    Nowe hasło zostało zapisane w schowku.
                    """);
    }

    private static Random random = new Random();

    public ILoggedInUserService LoggedInUserService { get; }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*:;,.<>";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private Task<bool> ConfirmUnsavedChanges()
    {
        return _dialogService.ShowConfirmation("Niezapisane zmiany", "Istnieją niezapisane zmiany.\nCzy kontynuować?");
    }

    public Task OnNavigatedFrom()
    {
        Users.Clear();
        SelectedUser = null;
        return Task.CompletedTask;
    }

    public async Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Loading users");

        IsLoading = true;

        var result = await _usersService.ListUsers();

        if (result.IsError)
        {
            var ex = result.UnwrapErr();
            _logger.Error(ex, "Error while loading users");
            await _loadingPopupService.Hide();
            await _dialogService.ShowAlertAsync("Błąd", ex.ToString());
            return;
        }

        var users = result.Unwrap().ToUserModels();

        foreach (var user in users.OrderBy(x => x.Name))
        {
            Users.Add(user);
        }

        OnPropertyChanged(nameof(Users));

        IsLoading = false;
    }
}
