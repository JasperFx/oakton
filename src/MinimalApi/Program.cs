using Microsoft.AspNetCore.Builder;
using Oakton;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
return app.RunOaktonCommandsSynchronously(args);