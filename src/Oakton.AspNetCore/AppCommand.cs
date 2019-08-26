using System;

namespace Oakton.AspNetCore
{
    [Oakton.Description("Special app command")]
    public class AppCommand : OaktonCommand<AspNetCoreInput>
    {
        public override bool Execute(AspNetCoreInput input)
        {
            Console.WriteLine("Hey, I'm running inside the app");
            return true;
        }
    }
}