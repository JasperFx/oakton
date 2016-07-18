using Baseline;
using System;

namespace Oakton
{
    public class ActivatorCommandCreator : ICommandCreator
    {
        public IOaktonCommand Create(Type commandType)
        {
            return Activator.CreateInstance(commandType).As<IOaktonCommand>();
        }
    }
}
