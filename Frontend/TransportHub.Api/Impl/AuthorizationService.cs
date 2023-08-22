using Lindronics.OneOf.Result;
using System.Net.Http.Headers;
using System.Text.Json;
using TransportHub.Api.Dtos;
using TransportHub.Api.Exceptions;
using TransportHub.Common;
using TransportHub.Services;

namespace TransportHub.Api.Impl;

public class AuthorizationService : IAuthorizationService
{
    private readonly ISettingsService _settingsService;
    private readonly IHttpClientFactory _httpClientFactory;
    private LoginResponseDto? _userData;

    public AuthorizationService(
        ISettingsService settingsService,
        IHttpClientFactory httpClientFactory)
    {
        _settingsService = settingsService;
        _httpClientFactory = httpClientFactory;
    }
    public bool IsLoggedIn => false;

    public event Action<LoginResponseDto>? LoggedIn;
    public event Action? LoggedOut;

    public async Task<Result<LoginResponseDto, Exception>> Login(string? login, string? password, bool closeOtherSessions)
    {
        if (login is null)
            return new ArgumentNullException(nameof(login));

        if (password is null)
            return new ArgumentNullException(nameof(password));


        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);
        var client = _httpClientFactory.Create(uri);

        var dto = new LoginRequestDto
        {
            User = login,
            Password = password,
            DisconnectOtherSessions = closeOtherSessions
        };


        var jsonResult = Throwable.ToResult(
            () => JsonSerializer.Serialize(dto, Serializer.GetSerializerOptions()));

        if (jsonResult.IsError)
            return jsonResult.UnwrapErr();

        var json = jsonResult.Unwrap();

        var response = await Throwable.ToResultAsync(
            async () => await client.PostAsync(
                "auth/login",
                new StringContent(json, MediaTypeHeaderValue.Parse("application/json"))));

        if (response.IsError)
            return response.UnwrapErr();

        var message = response.Unwrap();

        var content = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            return new LoginFailedException(content);
        }

        var res = Throwable.ToResult(() => JsonSerializer.Deserialize<LoginResponseDto>(content, Serializer.GetSerializerOptions()));

        return res.Match<Result<LoginResponseDto, Exception>>(
            ok =>
            {
                if (ok is not null)
                {
                    LoggedIn?.Invoke(ok);
                    _userData = ok;
                    return ok;
                }

                return new InvalidResponseException();
            },
            err => err);
    }

    public async Task<Result<bool, Exception>> Logout()
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);
        var client = _httpClientFactory.Create(uri, _userData?.User, _userData?.Key);

        var response = await Throwable.ToResultAsync(
            async () => await client.PostAsync(
                "auth/logout",
                new StringContent("")));

        if (response.IsError)
            return response.UnwrapErr();

        var message = response.Unwrap();

        var content = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            return new LogoutFailedException(content);
        }

        return true;
    }

    public Task<Result<bool, Exception>> RefreshSession()
    {
        throw new NotImplementedException();
    }
}
