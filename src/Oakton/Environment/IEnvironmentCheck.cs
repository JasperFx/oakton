using System;
using System.Threading;
using System.Threading.Tasks;

namespace Oakton.Environment;

#nullable disable annotations // FIXME

#region sample_IEnvironmentCheck

/// <summary>
///     Executed during bootstrapping time to carry out environment tests
///     against the application
/// </summary>
public interface IEnvironmentCheck
{
    /// <summary>
    ///     A textual description for command line output that describes
    ///     what is being checked
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     Asserts that the current check is valid. Throw an exception
    ///     to denote a failure
    /// </summary>
    Task Assert(IServiceProvider services, CancellationToken cancellation);
}

#endregion