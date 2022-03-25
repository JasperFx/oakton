using System;
using Oakton;

#region sample_using_OaktonCommandAssemblyAttribute
[assembly:Oakton.OaktonCommandAssembly]
#endregion

namespace AspNetCoreExtensionCommands
{
    
    #region sample_SmokeCommand
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
    #endregion
}
