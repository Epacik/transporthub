using Frontend.Services.API;
using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Services.InMemory.API
{
    internal class InMemoryAuthorizationService : IAuthorizationService
    {
        public bool IsAuthorized { get; set; }

        public event EventHandler? Authorized;

        public async Task<Result<bool, Exception>> Authorize(string? login, string? password, bool closeOtherSessions)
        {
            await Task.Yield();
            if (string.IsNullOrWhiteSpace(login))
            {
                return new ArgumentException("Login nie może być pusty", nameof(login));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return new ArgumentException("Hasło nie może być puste", nameof(password));
            }

            IsAuthorized = true;
            Authorized?.Invoke(this, EventArgs.Empty);

            return true;


        }
    }
}
