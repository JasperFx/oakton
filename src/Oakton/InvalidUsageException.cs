using System;

namespace Oakton;

#nullable disable annotations // FIXME

public class InvalidUsageException : Exception
{
    public InvalidUsageException() : base(string.Empty)
    {
    }

    public InvalidUsageException(string message) : base(message)
    {
    }

    public InvalidUsageException(string message, Exception innerException) : base(message, innerException)
    {
    }
}