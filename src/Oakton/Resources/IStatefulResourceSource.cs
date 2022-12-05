using System.Collections.Generic;

namespace Oakton.Resources;

#region sample_IStatefulResourceSource

/// <summary>
///     Expose multiple stateful resources
/// </summary>
public interface IStatefulResourceSource
{
    IReadOnlyList<IStatefulResource> FindResources();
}

#endregion