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
    private readonly IAuthorizationService _authorizationService;
    public InMemoryUsersService(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }
    public Task<Result<bool, Exception>> Add(UserAddDto userDto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            StaticUsers.Add(new()
            {
                Id = $"ID_{userDto.Name}",
                Name = userDto.Name,
                Disabled = false,
                MultiLogin = userDto.MultiLogin,
                PasswordExpirationDate = userDto.PasswordExpirationDate,
                Picture = userDto.Picture,
                UserType = userDto.UserType,
            });

            return true;
        });
    }

    public Task<Result<UserDto, Exception>> GetUser(string id)
    {
        return Task.Run<Result<UserDto, Exception>>(() =>
        {
            var user = StaticUsers.Find(x => x.Id == id);
            if (user is null)
                return new InvalidOperationException("User not found");

            return user;
        });
    }

    public Task<Result<IEnumerable<UserDto>, Exception>> ListUsers()
    {
        return Task.FromResult((Result<IEnumerable<UserDto>, Exception>)StaticUsers);
    }

    public Task<Result<bool, Exception>> Remove(string id)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            var user = StaticUsers.Find(x => x.Id == id);

            if (user is not null)
            {
                StaticUsers.Remove(user);
            }

            return user is not null;
        });
    }

    public Task<Result<bool, Exception>> Update(string id, UserUpdateDto userDto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            var item = StaticUsers.Find(x => x.Id == id);
            if(item is null)
            {
                return new InvalidOperationException("User not found");
            }

            item.Name = userDto.Name;
            item.Picture = userDto.Picture;

            return true;
        });
    }

    public Task<Result<bool, Exception>> UpdateAsAdmin(string id, UserAdminUpdateDto userDto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            var currentUserId = _authorizationService.UserData?.User;
            if (!StaticUsers.Any(x => x.Id == currentUserId && x.UserType == UserType.Admin))
                return new InvalidOperationException("You have to be an admin");

            var item = StaticUsers.FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return new InvalidOperationException("User not found");
            }

            item.Name = userDto.Name;
            item.Picture = userDto.Picture;
            item.PasswordExpirationDate = userDto.PasswordExpirationDate;
            item.UserType = userDto.UserType;
            item.MultiLogin = userDto.MultiLogin;
            item.Disabled = userDto.Disabled;

            return true;
        });
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

    public Task<Result<bool, Exception>> UpdatePassword(string id, UserUpdatePasswordDto password)
    {
        return Task.FromResult<Result<bool, Exception>>(true);
    }
}
