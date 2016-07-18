using System;
using Oakton.Help;

namespace Oakton
{
    public interface IOaktonCommand
    {
        bool Execute(object input);
        Type InputType { get; }
        UsageGraph Usages { get; }
    }

    public interface IOaktonCommand<T> : IOaktonCommand
    {
        
    }
}