using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Oakton.Resources
{
    public interface IStatefulResourceSource
    {
        Task<IReadOnlyList<IStatefulResource>> FindResources(CancellationToken token);
    }
}