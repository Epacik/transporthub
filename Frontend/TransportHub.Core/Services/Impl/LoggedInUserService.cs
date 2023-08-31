using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using TransportHub.Api;
using TransportHub.Core.Mappers;
using TransportHub.Core.Models;

namespace TransportHub.Core.Services.Impl;

public partial class LoggedInUserService : ObservableObject, ILoggedInUserService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUsersService _usersService;
    private readonly IReportErrorService _reportErrorService;

    public LoggedInUserService(
        IAuthorizationService authorizationService,
        IUsersService usersService,
        IReportErrorService reportErrorService)
    {
        _authorizationService = authorizationService;
        _usersService = usersService;
        _reportErrorService = reportErrorService;

        _authorizationService.LoggedIn += AuthorizationService_LoggedIn;
        _authorizationService.LoggedOut += AuthorizationService_LoggedOut;
    }

    private void AuthorizationService_LoggedOut()
    {
        User = null;
    }

    private void AuthorizationService_LoggedIn(Api.Dtos.LoginResponseDto obj)
    {
        Task.Run(ForceRefreshAsync)
            .ContinueWith(async t =>
            {
                if (t.IsFaulted) await _reportErrorService.ShowError(t.Exception!);
            });
    }

    [ObservableProperty]
    private UserModel? _user;

    public async Task ForceRefreshAsync()
    {
        if (_authorizationService?.UserData?.User is null)
            return;

        var result = await _usersService.GetUser(_authorizationService.UserData.User);

        if (result.IsError)
        {
            await _reportErrorService.ShowError(result.UnwrapErr());
            return;
        }

        User = result.Unwrap().ToUserModel();
    }
}
