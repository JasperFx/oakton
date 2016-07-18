using System.Collections.Generic;

namespace Oakton
{
    public interface ITokenHandler
    {
        bool Handle(object input, Queue<string> tokens);

        string ToUsageDescription();
        string Description { get; }
        string PropertyName { get; }
    }
}