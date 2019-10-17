using System;
using Oakton.AspNetCore.Html;
using Xunit;

namespace Oakton.AspNetCore.Testing.Html
{
    public class HtmlTagSmokeTests
    {
        // All of this code was ripped off from HtmlTags and it's been stable for *years*



        [Fact]
        public void smoke_test_document_with_tags()
        {
            var document = new HtmlDocument();

            document.Body.Add("h1").Text("Hello");
            
            var table = new TableTag();
            table.AddHeaderRow(row =>
            {
                row.Header("a");
                row.Header("b");
            });

            table.AddBodyRow(row =>
            {
                row.Cell("1");
                row.Cell("2");
            });
            
            Console.WriteLine(document.ToString());

        }
    }
}