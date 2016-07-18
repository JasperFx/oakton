using System.Collections.Generic;
using System.Reflection;

namespace Oakton
{
    public static class QueueExtensions
    {
        public static bool NextIsFlag(this Queue<string> queue)
        {
            return InputParser.IsFlag(queue.Peek());
        }

        public static bool NextIsFlagFor(this Queue<string> queue, PropertyInfo property)
        {
            return InputParser.IsFlagFor(queue.Peek(), property);
        }
    }
}