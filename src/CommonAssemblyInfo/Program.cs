using Oakton;

namespace CommonAssemblyInfo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var executor = CommandExecutor.For(_ => { _.RegisterCommand<AssemblyInfoCommand>(); });

            executor.OptionsFile = "assemblyinfo.opts";

            return executor.Execute(args);
        }
    }
}