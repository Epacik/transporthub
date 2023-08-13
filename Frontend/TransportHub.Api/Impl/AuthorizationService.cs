using Lindronics.OneOf.Result;
using TransportHub.Common;
using TransportHub.Services;

namespace TransportHub.Api.Impl;

public class AuthorizationService : IAuthorizationService
{
    private readonly ISettingsService _settingsService;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthorizationService(
        ISettingsService settingsService,
        IHttpClientFactory httpClientFactory)
    {
        _settingsService = settingsService;
        _httpClientFactory = httpClientFactory;
    }
    public bool IsLoggedIn => false;

    public event Action? LoggedIn;
    public event Action? LoggedOut;

    public async Task<Result<bool, Exception>> Login(string? login, string? password, bool closeOtherSessions)
    {
        var uri = _settingsService.Read(Settings.IpAddress, "127.0.0.1");
        var client = _httpClientFactory.Create();

        return false;
    }

    public Task<Result<bool, Exception>> Logout()
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool, Exception>> RefreshSession()
    {
        throw new NotImplementedException();
    }
}
