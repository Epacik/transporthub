using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    public Task<Result<bool, Exception>> Add(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserDto, Exception>> GetUser(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<UserDto>, Exception>> ListUsers()
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);

        var userdata = _authorizationService.UserData;
        var client = _httpClientFactory.Create(uri, userdata?.User, password: userdata?.Key);

        var response = await Throwable.ToResultAsync(
           async () => await client.GetAsync("users/list"));

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

    public Task<Result<bool, Exception>> Remove(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool, Exception>> Update(UserDto userDto)
    {
        throw new NotImplementedException();
    }
}
