using System;
using Oakton;

// SAMPLE: using-OaktonCommandAssemblyAttribute
[assembly:Oakton.OaktonCommandAssembly]
// ENDSAMPLE

namespace AspNetCoreExtensionCommands
{
    
    // SAMPLE: SmokeCommand
    [Description("Simply try to build a web host as a smoke test", Name = "smoke")]
    public class SmokeCommand : OaktonCommand<NetCoreInput>
    {
        public override bool Execute(NetCoreInput input)
        {
            // This method builds out the IWebHost for your
            // configured IHostBuilder of the application
            using (var host = input.BuildHost())
            {
                Console.WriteLine("It's all good");
            }

            return true;
        }
    }
    // ENDSAMPLE
}