using System;
using ExtensionCommands;
using Microsoft.Extensions.DependencyInjection;
using Oakton;
using Oakton.Resources;

[assembly:OaktonCommandAssembly(typeof(ExtensionServices))]

namespace ExtensionCommands
{
    public class ExtensionServices : IServiceRegistrations
    {
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<IExtensionService, ExtensionService>();
            services.AddSingleton<IStatefulResource>(new ExtensionResource());
        }
    }
    
    public interface IExtensionService{}
    public class ExtensionService : IExtensionService{}
    
    public class ExtensionInput
    {
        
    }

    public class ExtensionResource : StatefulResourceBase
    {
        public ExtensionResource() : base("Extension", "The Extension")
        {
        }
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