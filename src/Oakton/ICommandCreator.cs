using System;

namespace Oakton
{
    public interface ICommandCreator
    {
        IOaktonCommand Create(Type commandType);
    }
}
