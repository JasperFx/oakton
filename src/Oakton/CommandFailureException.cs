using System;

namespace Oakton;

#nullable disable annotations // FIXME

public class CommandFailureException : Exception
{
    public CommandFailureException(string message) : base(message)
    {
    }
}