using Oakton.Html;

namespace Oakton.Descriptions
{
    /// <summary>
    /// Optional interface that can be extended by classes using IDescribedSystemPart
    /// for better HTML report generation
    /// </summary>
    public interface IDescribedByHtml
    {
        HtmlTag Build();
    }
}