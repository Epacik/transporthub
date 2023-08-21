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

    public async Task<Result<LoginResponseDto, Exception>> Login(string? login, string? password, bool closeOtherSessions)
    {
        if (login is null)
            return new ArgumentNullException(nameof(login));

        if (password is null)
            return new ArgumentNullException(nameof(password));


        var uri = _settingsService.Read(Settings.IpAddress, "127.0.0.1");
        var client = _httpClientFactory.Create($"http://{uri}/api/v1/");

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

        var res = Throwable.ToResult(() => JsonSerializer.Deserialize<LoginResponseDto>(content));

        return res.Match<Result<LoginResponseDto, Exception>>(
            ok =>
            {
                if (ok is not null)
                    return ok;

                return new InvalidResponseException();
            },
            err => err);
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
