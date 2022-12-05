namespace Oakton.Environment;

public interface IEnvironmentCheckFactory
{
    IEnvironmentCheck[] Build();
}