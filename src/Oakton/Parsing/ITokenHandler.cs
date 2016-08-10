using System.Collections.Generic;

namespace Oakton.Parsing
{
    public interface ITokenHandler
    {
        bool Handle(object input, Queue<string> tokens);

        string ToUsageDescription();
        string Description { get; }
        string MemberName { get; }
    }
}