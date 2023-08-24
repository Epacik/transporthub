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
    private readonly IJsonSerializer _jsonSerializer;
    public LoginResponseDto? UserData { get; private set; }

    private PeriodicTimer _periodicTimer;
    private CancellationTokenSource? _cancellationTokenSource;

    public AuthorizationService(
        ISettingsService settingsService,
        IHttpClientFactory httpClientFactory,
        IJsonSerializer jsonSerializer)
    {
        _settingsService = settingsService;
        _httpClientFactory = httpClientFactory;
        _jsonSerializer = jsonSerializer;

        _periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(10));
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


        var jsonResult = _jsonSerializer.Serialize(dto);

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

        var res = _jsonSerializer.Deserialize<LoginResponseDto>(content);

        return res.Match<Result<LoginResponseDto, Exception>>(
            ok =>
            {
                if (ok is not null)
                {
                    LoggedIn?.Invoke(ok);
                    UserData = ok;

                    StartRefreshing();
                    
                    return ok;
                }

                return new InvalidResponseException();
            },
            err => err);
    }

    public async Task<Result<bool, Exception>> Logout()
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);
        var client = _httpClientFactory.Create(uri, UserData?.User, UserData?.Key);

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

        LoggedOut?.Invoke();
        _cancellationTokenSource?.Cancel();

        return true;
    }

    public async Task<Result<bool, Exception>> RefreshSession()
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);
        var client = _httpClientFactory.Create(uri, UserData?.User, UserData?.Key);

        var response = await Throwable.ToResultAsync(
           async () => await client.PostAsync(
               "auth/refresh-token",
               new StringContent("")));

        if (response.IsError)
            return response.UnwrapErr();

        var message = response.Unwrap();

        var content = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            return new RefreshTokenFailedException(content);
        }

        return true;
    }

    private void StartRefreshing()
    {
        async Task run()
        {
            while(_cancellationTokenSource?.IsCancellationRequested == false)
            {
                try
                {
                    await _periodicTimer.WaitForNextTickAsync(_cancellationTokenSource.Token);
                    await RefreshSession();
                    Console.Write("refreshed");
                }
                catch (OperationCanceledException)
                {
                    //swallow
                    return;
                }
            }
        }

        _cancellationTokenSource = new CancellationTokenSource();

        new Thread(async () => await run())
            .Start();
    }
}
