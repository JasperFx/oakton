// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Oakton;

return Host.CreateDefaultBuilder(args)
    .RunOaktonCommandsSynchronously(args, "sample.opts");