using System;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
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
    }
    
    // SAMPLE: git-commands
    [Description("Switch branches or restore working tree files")]
    public class CheckoutCommand : OaktonCommand<CheckoutInput>
    {
        public override bool Execute(CheckoutInput input)
        {
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
