using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;

namespace TransportHub.Api;

public interface IAuthorizationService
{
    bool IsLoggedIn { get; }

    event Action<LoginResponseDto>? LoggedIn;
    event Action? LoggedOut;

    Task<Result<LoginResponseDto, Exception>> Login(string? login, string? password, bool closeOtherSessions);
    Task<Result<bool, Exception>> RefreshSession();
    Task<Result<bool, Exception>> Logout();
}
