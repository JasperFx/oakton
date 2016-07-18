using System.Collections.Generic;
using System.Reflection;

namespace Oakton
{
    public interface ICommandFactory
    {
        CommandRun BuildRun(string commandLine);
        CommandRun BuildRun(IEnumerable<string> args);
        void RegisterCommands(Assembly assembly);

        IEnumerable<IOaktonCommand> BuildAllCommands();
    }
}