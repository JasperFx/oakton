using Oakton;

namespace CommonAssemblyInfo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return CommandExecutor.ExecuteCommand<AssemblyInfoCommand>(args, "assemblyinfo.opts");
        }
    }
}