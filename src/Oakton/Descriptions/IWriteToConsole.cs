using System.Threading.Tasks;

namespace Oakton.Descriptions;

#nullable disable annotations // FIXME

/// <summary>
///     Optional interface for exposing specialized console output
///     in the "describe" command
/// </summary>
public interface IWriteToConsole
{
    Task WriteToConsole();
}