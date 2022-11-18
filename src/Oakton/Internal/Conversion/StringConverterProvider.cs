using System;
using System.Linq.Expressions;
using JasperFx.Reflection;

namespace Oakton.Internal.Conversion
{
    public class StringConverterProvider : IConversionProvider
    {
        public Func<string, object>? ConverterFor (Type type)
        {
            if (!type.IsConcrete()) {
                return null;
            }
            var constructor = type.GetConstructor (new [] { typeof(string) });
            if (constructor == null) {
                return null;
            }

            var param = Expression.Parameter (typeof(string), "arg");
            var body = Expression.New (constructor, param);

            return Expression.Lambda<Func<string, object>> (body, param).Compile();
        }
    }
}

