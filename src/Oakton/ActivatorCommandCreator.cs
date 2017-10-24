using Baseline;
using System;

namespace Oakton
{
    public class ActivatorCommandCreator : ICommandCreator
    {
        public IOaktonCommand CreateCommand(Type commandType)
        {
            return Activator.CreateInstance(commandType).As<IOaktonCommand>();
        }

        public object CreateModel(Type modelType)
        {
            return Activator.CreateInstance(modelType);
        }
    }
}
