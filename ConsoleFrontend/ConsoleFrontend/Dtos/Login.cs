using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFrontend.Dtos
{
    internal class LoginRequest
    {
        public LoginRequest(string user, string password, bool? disconnectOtherSessions = null)
        {
            User = user;
            Password = password;
            DisconnectOtherSessions = disconnectOtherSessions;
        }

        public string? User { get; set; }
        public string? Password { get; set; }
        public bool? DisconnectOtherSessions { get; set; }
    }
}
