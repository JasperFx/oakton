using System.Threading;
using System.Threading.Tasks;

namespace Oakton.Resources
{
    public interface IStatefulResource
    {
        Task<object> Check(CancellationToken token);
        Task<object> ClearState(CancellationToken token);
        Task<object> Teardown(CancellationToken token);
        Task<object> Setup(CancellationToken token);
        Task<object> WriteStatus();
        
        string Type { get; }
        string Name { get; }
    }
}