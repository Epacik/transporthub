using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api;

public interface IAuthorizationService
{
    bool IsLoggedIn { get; }

    event Action? LoggedIn;
    event Action? LoggedOut;

    Task<Result<bool, Exception>> Login(string? login, string? password, bool closeOtherSessions);
    Task<Result<bool, Exception>> RefreshSession();
    Task<Result<bool, Exception>> Logout();
}
