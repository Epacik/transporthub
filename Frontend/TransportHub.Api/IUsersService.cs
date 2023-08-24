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
    Task<Result<bool, Exception>> Add(UserDto userDto);
    Task<Result<bool, Exception>> Remove(string id);
    Task<Result<bool, Exception>> Update(UserDto userDto);
}
