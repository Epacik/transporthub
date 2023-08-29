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

internal partial class SettingsViewModel : ObservableObject, INavigationAwareViewModel
{
    private readonly IUsersService _usersService;
    private readonly ILogger _logger;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IDialogService _dialogService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IReportErrorService _reportErrorService;
    private readonly IUserProvidedImageService _userProvidedImageService;
    private readonly IRefreshUserDataService _refreshUserDataService;

    public SettingsViewModel(
        IUsersService usersService,
        ILogger logger,
        ILoadingPopupService loadingPopupService,
        IDialogService dialogService,
        IAuthorizationService authorizationService,
        IReportErrorService reportErrorService,
        IUserProvidedImageService userProvidedImageService,
        IRefreshUserDataService refreshUserDataService)
    {
        _usersService = usersService;
        _logger = logger;
        _loadingPopupService = loadingPopupService;
        _dialogService = dialogService;
        _authorizationService = authorizationService;
        _reportErrorService = reportErrorService;
        _userProvidedImageService = userProvidedImageService;
        _refreshUserDataService = refreshUserDataService;
    }

    private UserDto _user;

    [ObservableProperty]
    private UserModel? _editedUser;
    

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
        await _loadingPopupService.Hide();
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
            EditedUser = _user.ToUserModel();
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        await _loadingPopupService.Show("Zapisywanie zmian");
        var result = await _usersService.UpdateAsAdmin(EditedUser!.Id!, EditedUser!.ToUserAdminUpdateDto());

        if (result.IsError)
        {
            await _loadingPopupService.Hide();
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        var clone = EditedUser!.Clone();
        await _loadingPopupService.Hide();

        _refreshUserDataService.RequestRefresh();

    }

}
