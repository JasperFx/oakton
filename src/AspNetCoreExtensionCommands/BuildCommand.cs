using System;
using Oakton;
using Oakton.AspNetCore;

[assembly:OaktonCommandAssembly]

namespace AspNetCoreExtensionCommands
{
    
    
    [Description("Simply try to build a web host as a smoke test", Name = "smoke")]
    public class SmokeCommand : OaktonCommand<AspNetCoreInput>
    {
        public override bool Execute(AspNetCoreInput input)
        {
            using (var host = input.BuildHost())
            {
                Console.WriteLine("It's all good");
            }

            return true;
        }
    }
}