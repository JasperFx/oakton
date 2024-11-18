using System.IO;
using System.Threading.Tasks;

namespace Oakton.Descriptions;

#nullable disable annotations // FIXME

#region sample_IDescribedSystemPart

/// <summary>
///     Base class for a "described" part of your application.
///     Implementations of this type should be registered in your
///     system's DI container to be exposed through the "describe"
///     command
/// </summary>
public interface IDescribedSystemPart
{
    /// <summary>
    ///     A descriptive title to be shown in the rendered output
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     Write markdown formatted text to describe this system part
    /// </summary>
    /// <param name="writer"></param>
    /// <returns></returns>
    Task Write(TextWriter writer);
}

#endregion