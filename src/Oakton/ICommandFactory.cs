using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Oakton;

#nullable disable annotations // FIXME

/// <summary>
///     Interface that Oakton uses to build command runs during execution. Can be used for custom
///     command activation
/// </summary>
public interface ICommandFactory
{
    CommandRun BuildRun(string commandLine);
    CommandRun BuildRun(IEnumerable<string> args);
    void RegisterCommands(Assembly assembly);

    IEnumerable<IOaktonCommand> BuildAllCommands();

    void ApplyExtensions(IHostBuilder builder);
}