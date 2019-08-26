using System;
using Oakton;

[assembly:OaktonCommandAssembly]

namespace ExtensionCommands
{
    public class ExtensionInput
    {
        
    }
    
    [Description("An extension command loaded from another assembly", Name = "extension")]
    public class ExtensionCommand : OaktonCommand<ExtensionInput>
    {
        public override bool Execute(ExtensionInput input)
        {
            Console.WriteLine("I'm an extension command");
            return true;
        }
    }
    
    [Description("A second extension command loaded from another assembly", Name = "extension2")]
    public class Extension2Command : OaktonCommand<ExtensionInput>
    {
        public override bool Execute(ExtensionInput input)
        {
            Console.WriteLine("I'm an extension command");
            return true;
        }
    }
}