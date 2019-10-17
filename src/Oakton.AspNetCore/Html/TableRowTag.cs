namespace Oakton.AspNetCore.Html
{
    public class TableRowTag : HtmlTag
    {
        public TableRowTag()
            : base("tr")
        {
        }

        public HtmlTag Header(string text) => new HtmlTag("th", this).Text(text);

        public HtmlTag Header() => new HtmlTag("th", this);

        public HtmlTag Cell(string text) => new HtmlTag("td", this).Text(text);

        public HtmlTag Cell() => new HtmlTag("td", this);
    }
}