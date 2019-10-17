using Oakton.AspNetCore.Html;

namespace Oakton.AspNetCore.Descriptions
{
    /// <summary>
    /// Optional interface to customize the appearance of an exported HTML
    /// report built by the "describe" command
    /// </summary>
    public interface IDescribedHtmlFormatter
    {
        void ApplyFormatting(HtmlDocument document);
    }
}