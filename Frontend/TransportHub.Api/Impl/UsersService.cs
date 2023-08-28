using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;
using TransportHub.Api.Exceptions;
using TransportHub.Common;
using TransportHub.Services;

namespace TransportHub.Api.Impl;

public class UsersService : IUsersService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ISettingsService _settingsService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJsonSerializer _jsonSerializer;

    private readonly MediaTypeHeaderValue _mediaTypeHeaderValue;

    public UsersService(
        IAuthorizationService authorizationService,
        ISettingsService settingsService,
        IHttpClientFactory httpClientFactory,
        IJsonSerializer jsonSerializer)
    {
        _authorizationService = authorizationService;
        _settingsService = settingsService;
        _httpClientFactory = httpClientFactory;
        _jsonSerializer = jsonSerializer;

        _mediaTypeHeaderValue = MediaTypeHeaderValue.Parse("application/json");
    }

    public async Task<Result<bool, Exception>> Add(UserAddDto userDto)
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);

        var userdata = _authorizationService.UserData;
        var client = _httpClientFactory.Create(uri!, userdata?.User, password: userdata?.Key);

        var data = _jsonSerializer.Serialize(userDto);

        if (data.IsError)
            return data.UnwrapErr();

        var response = await client.PutAsync(
                "users/admin/add",
                new StringContent(data.Unwrap(), _mediaTypeHeaderValue))
            .ToResultAsync();

        var unwrapped = response.Unwrap();

        if (!unwrapped.IsSuccessStatusCode)
        {
            var contentResult = await unwrapped.Content.ReadAsStringAsync().ToResultAsync();

            if (contentResult.IsError)
                return contentResult.UnwrapErr();

            var content = contentResult.Unwrap();

            return new InvalidOperationException(content);
        }

        return true;
    }

    public Task<Result<UserDto, Exception>> GetUser(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<UserDto>, Exception>> ListUsers()
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);

        var userdata = _authorizationService.UserData;
        var client = _httpClientFactory.Create(uri!, userdata?.User, password: userdata?.Key);

        var response = await client.GetAsync("users/list").ToResultAsync();

        if (response.IsError)
            return response.UnwrapErr();

        var message = response.Unwrap();

        var content = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            return new RefreshTokenFailedException(content);
        }


        var users = _jsonSerializer.Deserialize<IEnumerable<UserDto>>(content);

        return users!;
    }

    public async Task<Result<bool, Exception>> Remove(string id)
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);

        var userdata = _authorizationService.UserData;
        var client = _httpClientFactory.Create(uri!, userdata?.User, password: userdata?.Key);


        var response = await client.DeleteAsync(
                $"users/{id}/admin/delete")
            .ToResultAsync();

        var unwrapped = response.Unwrap();

        if (!unwrapped.IsSuccessStatusCode)
        {
            var contentResult = await unwrapped.Content.ReadAsStringAsync().ToResultAsync();

            if (contentResult.IsError)
                return contentResult.UnwrapErr();

            var content = contentResult.Unwrap();

            return new InvalidOperationException(content);
        }

        return true;
    }

    public async Task<Result<bool, Exception>> Update(string id, UserUpdateDto userDto)
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);

        var userdata = _authorizationService.UserData;
        var client = _httpClientFactory.Create(uri!, userdata?.User, password: userdata?.Key);

        var data = _jsonSerializer.Serialize(userDto);

        if (data.IsError)
            return data.UnwrapErr();

        var response = await client.PatchAsync(
                $"users/{id}/update",
                new StringContent(data.Unwrap()!, _mediaTypeHeaderValue))
            .ToResultAsync();

        if (response.IsError)
            return response.UnwrapErr();

        var unwrapped = response.Unwrap();

        if (!unwrapped.IsSuccessStatusCode)
        {
            var contentResult = await unwrapped.Content.ReadAsStringAsync().ToResultAsync();

            if (contentResult.IsError)
                return contentResult.UnwrapErr();

            var content = contentResult.Unwrap();

            return new InvalidOperationException(content);
        }

        return true;
    }

    public async Task<Result<bool, Exception>> UpdateAsAdmin(string id, UserAdminUpdateDto userDto)
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);

        var userdata = _authorizationService.UserData;
        var client = _httpClientFactory.Create(uri!, userdata?.User, password: userdata?.Key);

        var data = _jsonSerializer.Serialize(userDto);

        if (data.IsError)
            return data.UnwrapErr();

        var response = await client.PatchAsync(
                $"users/{id}/admin/update",
                new StringContent(data.Unwrap()!, MediaTypeHeaderValue.Parse("application/json")))
            .ToResultAsync();

        if (response.IsError)
            return response.UnwrapErr();

        var unwrapped = response.Unwrap();

        if(!unwrapped.IsSuccessStatusCode)
        {
            var contentResult = await unwrapped.Content.ReadAsStringAsync().ToResultAsync();

            if (contentResult.IsError)
                return contentResult.UnwrapErr();

            var content = contentResult.Unwrap();

            return new InvalidOperationException(content);
        }

        return true;
    }

    public async Task<Result<bool, Exception>> UpdatePassword(string id, UserUpdatePasswordDto userUpdatePasswordDto)
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);

        var userdata = _authorizationService.UserData;
        var client = _httpClientFactory.Create(uri!, userdata?.User, password: userdata?.Key);

        var data = _jsonSerializer.Serialize(userUpdatePasswordDto);

        if (data.IsError)
            return data.UnwrapErr();

        var response = await client.PatchAsync(
                $"users/{id}/update_password",
                new StringContent(data.Unwrap()!, _mediaTypeHeaderValue))
            .ToResultAsync();

        if (response.IsError)
            return response.UnwrapErr();

        var unwrapped = response.Unwrap();

        if (!unwrapped.IsSuccessStatusCode)
        {
            var contentResult = await unwrapped.Content.ReadAsStringAsync().ToResultAsync();

            if (contentResult.IsError)
                return contentResult.UnwrapErr();

            var content = contentResult.Unwrap();

            return new InvalidOperationException(content);
        }

        return true;
    }
}
