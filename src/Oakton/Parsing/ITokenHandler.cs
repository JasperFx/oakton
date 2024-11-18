using System.Collections.Generic;

namespace Oakton.Parsing;

#nullable disable annotations // FIXME

public interface ITokenHandler
{
    string Description { get; }
    string MemberName { get; }
    bool Handle(object input, Queue<string> tokens);

    string ToUsageDescription();
}