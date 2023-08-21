using TransportHub.Api;
using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;

namespace TransportHub.Services.InMemory.API
{
    internal class InMemoryAuthorizationService : IAuthorizationService
    {
        public bool IsLoggedIn { get; set; }

        public event Action? LoggedIn;
        public event Action? LoggedOut;

        public async Task<Result<LoginResponseDto, Exception>> Login(string? login, string? password, bool closeOtherSessions)
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

            IsLoggedIn = true;
            LoggedIn?.Invoke();

            return new LoginResponseDto
            {
                User = login,
                Key = "",
            };
        }

        public async Task<Result<bool, Exception>> Logout()
        {
            await Task.Delay(1000);
            IsLoggedIn = true;
            LoggedIn?.Invoke();
            return true;
        }

        public async Task<Result<bool, Exception>> RefreshSession()
        {
            await Task.Delay(50);
            return true;
        }
    }
}
