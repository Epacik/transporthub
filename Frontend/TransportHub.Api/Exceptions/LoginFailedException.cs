using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Exceptions;

public class LoginFailedException : Exception
{
    public LoginFailedException() : base()
    {
    }

    public LoginFailedException(string? message) : base(message)
    {
    }

    public LoginFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
