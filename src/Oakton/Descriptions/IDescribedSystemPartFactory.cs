namespace Oakton.Descriptions
{
    /// <summary>
    /// Optional interface to be registered to find described parts
    /// in the system
    /// </summary>
    public interface IDescribedSystemPartFactory
    {
        IDescribedSystemPart[] FindParts();
    }
}