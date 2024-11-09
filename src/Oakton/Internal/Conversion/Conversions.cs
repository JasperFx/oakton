using System;
using System.Collections.Generic;
using System.Linq;
using JasperFx.Core;

namespace Oakton.Internal.Conversion;

public class Conversions
{
#nullable disable annotations // FIXME
    private readonly LightweightCache<Type, Func<string, object>> _convertors;
    private readonly IList<IConversionProvider> _providers = new List<IConversionProvider>();
#nullable restore

    public Conversions()
    {
        _convertors =
            new LightweightCache<Type, Func<string, object?>?>(
                type => { return providers().Select(x => x.ConverterFor(type)).FirstOrDefault(x => x != null); });

        RegisterConversion(bool.Parse);
        RegisterConversion(byte.Parse);
        RegisterConversion(sbyte.Parse);
        RegisterConversion(char.Parse);
        RegisterConversion(decimal.Parse);
        RegisterConversion(double.Parse);
        RegisterConversion(float.Parse);
        RegisterConversion(short.Parse);
        RegisterConversion(int.Parse);
        RegisterConversion(long.Parse);
        RegisterConversion(ushort.Parse);
        RegisterConversion(uint.Parse);
        RegisterConversion(ulong.Parse);
        RegisterConversion(DateTimeConverter.GetDateTime);
        RegisterConversion(Guid.Parse);

        RegisterConversion(x =>
        {
            if (x == "EMPTY")
            {
                return string.Empty;
            }

            return x;
        });
    }


    private IEnumerable<IConversionProvider> providers()
    {
        foreach (var provider in _providers) yield return provider;

        yield return new EnumerationConversion();
        yield return new NullableConvertor(this);
        yield return new ArrayConversion(this);
        yield return new StringConverterProvider();
    }

    public void RegisterConversionProvider<T>() where T : IConversionProvider, new()
    {
        _providers.Add(new T());
    }

    public void RegisterConversion<T>(Func<string, T> convertor)
    {
        _convertors[typeof(T)] = x => convertor(x);
    }

    public Func<string, object?>? FindConverter(Type type)
    {
        return _convertors[type];
    }

    public object? Convert(Type type, string raw)
    {
        return _convertors[type]?.Invoke(raw);
    }

    public bool Has(Type type)
    {
        return _convertors.Contains(type) || providers().Any(x => x.ConverterFor(type) != null);
    }
}