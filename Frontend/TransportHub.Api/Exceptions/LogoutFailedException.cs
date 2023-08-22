using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Exceptions;

public class LogoutFailedException : Exception
{
    public LogoutFailedException() : base()
    {
    }

    public LogoutFailedException(string? message) : base(message)
    {
    }

    public LogoutFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
