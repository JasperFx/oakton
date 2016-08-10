using System;

namespace Oakton
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreOnCommandLineAttribute : Attribute
    {
        
    }
}