using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;

namespace TransportHub.Api;

public interface IUsersService
{
    Task<Result<IEnumerable<UserDto>, Exception>> ListUsers();
    Task<Result<UserDto, Exception>> GetUser(string id);
    Task<Result<bool, Exception>> Add(UserAddDto userDto);
    Task<Result<bool, Exception>> Remove(string id);
    Task<Result<bool, Exception>> Update(string id, UserUpdateDto userDto);
    Task<Result<bool, Exception>> UpdateAsAdmin(string id, UserAdminUpdateDto userDto);
    Task<Result<bool, Exception>> UpdatePassword(string id, UserUpdatePasswordDto userUpdatePasswordDto);
}
