using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Oakton.Html
{
    public class HtmlDocument
    {
        private readonly List<Func<string, string>> _alterations = new List<Func<string, string>>();

        private readonly Stack<HtmlTag> _currentStack = new Stack<HtmlTag>();
        private readonly HtmlTag _title;

        public HtmlDocument()
        {
            RootTag = new HtmlTag("html");
            DocType = "<!DOCTYPE html>";
            Head = RootTag.Add("head");
            _title = Head.Add("title");
            Body = RootTag.Add("body");
            Last = Body;
        }

        public string DocType { get; set; }
        public HtmlTag RootTag { get; }
        public HtmlTag Head { get; }
        public HtmlTag Body { get; }
        public string Title { get { return _title.Text(); } set { _title.Text(value); } }

        public HtmlTag Current => _currentStack.Any() ? _currentStack.Peek() : Body;
        public HtmlTag Last { get; private set; }

        private static void EnsureFolderExists(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);

            if (string.IsNullOrEmpty(folder))
            {
                return;
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public HtmlTag Add(string tagName)
        {
            Last = Current.Add(tagName);
            return Last;
        }

        public void Add(HtmlTag tag)
        {
            Last = tag;
            Current.Append(tag);
        }

        public HtmlTag Push(string tagName)
        {
            var tag = Add(tagName);
            _currentStack.Push(tag);

            return tag;
        }

        public void Push(HtmlTag tag)
        {
            Current.Append(tag);
            _currentStack.Push(tag);
        }

        public void PushWithoutAttaching(HtmlTag tag) => _currentStack.Push(tag);

        public void Pop()
        {
            if (_currentStack.Any())
            {
                _currentStack.Pop();
            }
        }

        private string Substitute(string value) => _alterations.Aggregate(value, (current, alteration) => alteration(current));

        public override string ToString()
        {
            var value = DocType + System.Environment.NewLine + RootTag;
            return Substitute(value);
        }

        public HtmlTag AddStyle(string styling)
        {
            var key = Guid.NewGuid().ToString();
            _alterations.Add(html => html.Replace(key, styling));
            return Head.Add("style").Text(key);
        }

        public HtmlTag AddJavaScript(string javascript) => AddScript("text/javascript", javascript);

        public HtmlTag AddScript(string scriptType, string scriptContents)
        {
            var key = Guid.NewGuid().ToString();
            _alterations.Add(html => html.Replace(key, System.Environment.NewLine + scriptContents + System.Environment.NewLine));
            return Head.Add("script").Attr("type", scriptType).Text(key);
        }

        public HtmlTag ReferenceJavaScriptFile(string path) => ReferenceScriptFile("text/javascript", path);

        public HtmlTag ReferenceScriptFile(string scriptType, string path) => Head.Add("script").Attr("type", scriptType).Attr("src", path);

        public HtmlTag ReferenceStyle(string path)
        {
            return Head.Add("link")
                .Attr("media", "screen")
                .Attr("href", path)
                .Attr("type", "text/css")
                .Attr("rel", "stylesheet");
        }

        public void Rewind() => _currentStack.Clear();
    }
}