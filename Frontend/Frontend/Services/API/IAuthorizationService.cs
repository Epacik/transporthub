using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Services.API;

public interface IAuthorizationService
{
    bool IsAuthorized { get; }

    event EventHandler? Authorized;

    Task<Result<bool, Exception>> Authorize(string? login, string? password, bool closeOtherSessions);
}
