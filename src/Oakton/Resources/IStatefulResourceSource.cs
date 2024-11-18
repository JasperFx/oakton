using System.Collections.Generic;

namespace Oakton.Resources;

#nullable disable annotations // FIXME

#region sample_IStatefulResourceSource

/// <summary>
///     Expose multiple stateful resources
/// </summary>
public interface IStatefulResourceSource
{
    IReadOnlyList<IStatefulResource> FindResources();
}

#endregion