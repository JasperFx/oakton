#region sample_bootstrapping_minimal_api

using Oakton;

var builder = WebApplication.CreateBuilder(args);

// This isn't required, but it "helps" Oakton to enable
// some additional diagnostics for the stateful resource 
// model
builder.Host.ApplyOaktonExtensions();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

// Note the usage of await to force the implied
// Program.Main() method to be asynchronous
return await app.RunOaktonCommands(args);

#endregion