namespace Oakton.AspNetCore.Environment
{
    public interface IEnvironmentCheckFactory
    {
        IEnvironmentCheck[] Build();
    }
}