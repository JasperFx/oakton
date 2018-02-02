using System;
using System.Threading.Tasks;
using Oakton.Help;

namespace Oakton
{
    public interface IOaktonCommand
    {
        Task<bool> Execute(object input);
        Type InputType { get; }
        UsageGraph Usages { get; }
    }

    public interface IOaktonCommand<T> : IOaktonCommand
    {
        
    }
}