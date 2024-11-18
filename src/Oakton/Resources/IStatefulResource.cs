using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console.Rendering;

namespace Oakton.Resources;

#nullable disable annotations // FIXME

#region sample_IStatefulResourceWithDependencies

/// <summary>
/// Use to create dependencies between 
/// </summary>
public interface IStatefulResourceWithDependencies : IStatefulResource
{
    // Given all the known stateful resources in your system -- including the current resource!
    // tell Oakton which resources are dependencies of this resource that should be setup first
    IEnumerable<IStatefulResource> FindDependencies(IReadOnlyList<IStatefulResource> others);
}

#endregion

#region sample_IStatefulResource

/// <summary>
///     Adapter interface used by Oakton enabled applications to allow
///     Oakton to setup/teardown/clear the state/check on stateful external
///     resources of the system like databases or messaging queues
/// </summary>
public interface IStatefulResource
{
    /// <summary>
    ///     Categorical type name of this resource for filtering
    /// </summary>
    string Type { get; }

    /// <summary>
    ///     Identifier for this resource
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Check whether the configuration for this resource is valid. An exception
    ///     should be thrown if the check is invalid
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Check(CancellationToken token);

    /// <summary>
    ///     Clear any persisted state within this resource
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task ClearState(CancellationToken token);

    /// <summary>
    ///     Tear down the stateful resource represented by this implementation
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Teardown(CancellationToken token);

    /// <summary>
    ///     Make any necessary configuration to this stateful resource
    ///     to make the system function correctly
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Setup(CancellationToken token);

    /// <summary>
    ///     Optionally return a report of the current state of this resource
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IRenderable> DetermineStatus(CancellationToken token);
}

#endregion