using System;
using System.Reflection;
using Baseline.Reflection;

namespace Oakton
{
    // TODO -- move to Baseline
    public static class TypeExtensions
    {
        public static void ForAttribute<T>(this Type type, Action<T> action) where T : Attribute
        {
            type.GetTypeInfo().ForAttribute(action);
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetTypeInfo().HasAttribute<T>();
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetTypeInfo().GetAttribute<T>();
        }
    }
}