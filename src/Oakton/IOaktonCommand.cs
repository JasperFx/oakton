using System;
using System.Threading.Tasks;
using Oakton.Help;

namespace Oakton;

public interface IOaktonCommand
{
    Type InputType { get; }
    UsageGraph Usages { get; }
    Task<bool> Execute(object input);
}

public interface IOaktonCommand<T> : IOaktonCommand
{
}