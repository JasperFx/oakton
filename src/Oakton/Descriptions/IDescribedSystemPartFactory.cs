namespace Oakton.Descriptions;

#region sample_IDescribedSystemPartFactory

/// <summary>
///     Register implementations of this service to help
///     the describe command discover additional system parts
/// </summary>
public interface IDescribedSystemPartFactory
{
    IDescribedSystemPart[] Parts();
}

#endregion