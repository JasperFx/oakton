using System.Collections.Generic;

namespace Oakton.Descriptions;

/// <summary>
///     Interface to expose key/value pairs to diagnostic output
/// </summary>
public interface IDescribesProperties
{
    IDictionary<string, object> DescribeProperties();
}