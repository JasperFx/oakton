using System;

namespace Oakton.Internal.Conversion;

// SAMPLE: IConversionProvider
public interface IConversionProvider
{
    // Given the type argument, either return a
    // Func that can parse a string into that Type
    // or return null to let another IConversionProvider
    // handle this type
    Func<string, object?>? ConverterFor(Type type);
}
// ENDSAMPLE