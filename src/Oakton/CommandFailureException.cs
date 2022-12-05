using System;

namespace Oakton;

public class CommandFailureException : Exception
{
    public CommandFailureException(string message) : base(message)
    {
    }
}