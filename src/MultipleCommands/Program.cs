using System;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Oakton;

namespace MultipleCommands
{
    class Program
    {
        // SAMPLE: MultipleCommands.Program.Main
        static int Main(string[] args)
        {
            var executor = CommandExecutor.For(_ =>
            {
                // Find and apply all command classes discovered
                // in this assembly
                _.RegisterCommands(typeof(Program).GetTypeInfo().Assembly);
            });

            return executor.Execute(args);
        }
        // ENDSAMPLE

            /*
        // SAMPLE: MultipleCommands.Program.Main.Async
        static Task<int> Main(string[] args)
        {
            var executor = CommandExecutor.For(_ =>
            {
                // Find and apply all command classes discovered
                // in this assembly
                _.RegisterCommands(typeof(Program).GetTypeInfo().Assembly);
            });

            return executor.ExecuteAsync(args);
        }
        // ENDSAMPLE
        */
    }

    // SAMPLE: git-commands
    [Description("Switch branches or restore working tree files")]
    public class CheckoutCommand : OaktonAsyncCommand<CheckoutInput>
    {
        public override async Task<bool> Execute(CheckoutInput input)
        {
            await Task.CompletedTask;
            return true;
        }
    }
    
    [Description("Remove untracked files from the working tree")]
    public class CleanCommand : OaktonCommand<CleanInput>
    {
        public override bool Execute(CleanInput input)
        {
            return true;
        }
    }
    // ENDSAMPLE

    // SAMPLE: CheckoutInput
    public class CheckoutInput
    {
        [FlagAlias("create-branch",'b')]
        public string c { get; set; }
        
        public bool DetachFlag { get; set; }
        
        public bool ForceFlag { get; set; }
    }
    // ENDSAMPLE


    // SAMPLE: CleanInput
    public class CleanInput
    {
        [Description("Do it now!")]
        public bool ForceFlag { get; set; }
        
        [FlagAlias('d')]
        [Description("Remove untracked directories in addition to untracked files")]
        public bool RemoveUntrackedDirectoriesFlag { get; set; }
        
        [FlagAlias('x')]
        [Description("Remove only files ignored by Git")]
        public bool DoNoUseStandardIgnoreRulesFlag { get; set; }
    }
    // ENDSAMPLE


}
