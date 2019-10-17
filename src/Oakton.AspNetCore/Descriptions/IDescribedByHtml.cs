using Oakton.AspNetCore.Html;

namespace Oakton.AspNetCore.Descriptions
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