using System;
using System.Reflection;

namespace Oakton.Parsing
{
    public static class MemberInfoExtensions
    {
        public static Type GetMemberType(this MemberInfo member)
        {
            return (member as PropertyInfo)?.PropertyType ?? (member as FieldInfo)?.FieldType;
        }   
    }
}