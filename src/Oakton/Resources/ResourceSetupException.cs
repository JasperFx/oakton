using System;

namespace Oakton.Resources;

#nullable disable annotations // FIXME

public class ResourceSetupException : Exception
{
    public ResourceSetupException(IStatefulResource resource, Exception ex) : base(
        $"Failed to setup resource {resource.Name} of type {resource.Type}", ex)

    {
    }

    public ResourceSetupException(IStatefulResourceSource source, Exception ex) : base(
        $"Failed to execute resource source {source}", ex)
    {
    }
}