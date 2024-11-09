namespace Oakton.Environment;

#nullable disable annotations // FIXME

public interface IEnvironmentCheckFactory
{
    IEnvironmentCheck[] Build();
}