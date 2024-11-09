using System;

namespace Oakton.Descriptions;

#nullable disable annotations // FIXME

internal interface IRequiresServices
{
    void Resolve(IServiceProvider services);
}