using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Exceptions;

public class RefreshTokenFailedException : Exception
{
    public RefreshTokenFailedException() : base()
    {
    }

    public RefreshTokenFailedException(string? message) : base(message)
    {
    }

    public RefreshTokenFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
