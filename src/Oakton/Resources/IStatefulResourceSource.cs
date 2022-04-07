using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Oakton.Resources
{
    public interface IStatefulResourceSource
    {
        IReadOnlyList<IStatefulResource> FindResources();
    }
}