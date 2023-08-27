using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;
using TransportHub.Api.Dtos;

namespace TransportHub.Core.Services.InMemory.API;

public class InMemoryUsersService : IUsersService
{
    public Task<Result<bool, Exception>> Add(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserDto, Exception>> GetUser(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<UserDto>, Exception>> ListUsers()
    {
        return Task.FromResult((Result<IEnumerable<UserDto>, Exception>)StaticUsers);
    }

    public Task<Result<bool, Exception>> Remove(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool, Exception>> Update(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    internal static readonly List<UserDto> StaticUsers = new()
    {
        CreateUserDto("admin", null, UserType.Admin, false, false),
        CreateUserDto("Damian", null, UserType.Manager, true, false),
        CreateUserDto("Krystian", DateTime.Now.AddMonths(1), UserType.User, false, false),
        CreateUserDto("Sam", null, UserType.Manager, false, true),
        CreateUserDto("Grzegorz", DateTime.Now.AddDays(14), UserType.User, false, false),
    };

    private static UserDto CreateUserDto(
        string name,
        DateTime? passwordExpirationDate,
        UserType userType,
        bool multiLogin,
        bool disabled)
    {
        return new()
        {
            Id = $"ID_{name}",
            Name = name,
            PasswordExpirationDate = passwordExpirationDate,
            UserType = userType,
            MultiLogin = multiLogin,
            Disabled = disabled
        };
    }
}
