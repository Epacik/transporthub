using Frontend.Helpers;
using Frontend.Services.API;
using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Services.Impl.API;

internal class AuthorizationService : IAuthorizationService
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
    public bool IsAuthorized => false;

    public event EventHandler? Authorized;

    public async Task<Result<bool, Exception>> Authorize(string? login, string? password, bool closeOtherSessions)
    {
        var uri = _settingsService.Read(Settings.IpAddress, "127.0.0.1");
        var client = _httpClientFactory.Create();

        return false;
    }
}
