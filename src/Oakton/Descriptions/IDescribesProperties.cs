using System.Collections.Generic;

namespace Oakton.Descriptions;

#nullable disable annotations // FIXME

/// <summary>
///     Interface to expose key/value pairs to diagnostic output
/// </summary>
public interface IDescribesProperties
{
    IDictionary<string, object> DescribeProperties();
}