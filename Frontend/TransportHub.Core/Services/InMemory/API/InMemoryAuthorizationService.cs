using TransportHub.Api;
using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;
using TransportHub.Core.Services.InMemory.API;
using TransportHub.Api.Exceptions;

namespace TransportHub.Services.InMemory.API
{
    internal class InMemoryAuthorizationService : IAuthorizationService
    {
        public bool IsLoggedIn { get; set; }

        public LoginResponseDto? UserData { get; private set; }

        public event Action<LoginResponseDto>? LoggedIn;
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

            if (!InMemoryUsersService.StaticUsers.Any(x => x.Name == login))
            {
                return new LoginFailedException($"Nie znaleziono użytkownika \"{login}\"");
            }


            IsLoggedIn = true;

            var response = new LoginResponseDto
            {
                User = login,
                Key = "",
            };

            UserData = response;

            LoggedIn?.Invoke(response);

            return response;
        }

        public async Task<Result<bool, Exception>> Logout()
        {
            await Task.Delay(1000);
            IsLoggedIn = false;
            LoggedOut?.Invoke();
            return true;
        }

        public async Task<Result<bool, Exception>> RefreshSession()
        {
            await Task.Delay(50);
            return true;
        }
    }
}
