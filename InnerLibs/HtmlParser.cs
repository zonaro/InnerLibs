using System;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs
{
    internal class HtmlParser
    {
        private Queue<char> q;

        private char Dequeue() => q.Dequeue();

        private string Dequeue(int length)
        {
            string text = string.Empty;
            while (length-- > 0)
            {
                if (q.Any())
                    text += q.Dequeue();
            }
            return text;
        }

        private Dictionary<string, string> GetAttributes()
        {
            var attrs = new Dictionary<string, string>();

            if (!q.Any()) return attrs;

            SkipSpace();

            if (q.Peek() == '/') return attrs;

            do
            {
                attrs.SetOrRemove(GetAttribute());
                SkipSpace();
            } while (q.Any() && q.Peek() != '>' && q.Peek(2) != "/>");

            return attrs;

            KeyValuePair<string, string> GetAttribute()
            {
                var name = GetUpTo('=', ' ', '>');

                if (q.Peek() == ' ' || q.Peek() == '>') return new KeyValuePair<string, string>(name, null);

                Dequeue();

                if (q.Peek() == '>') return new KeyValuePair<string, string>(name, null);

                if (q.Peek(2) == "''" || q.Peek(2) == "\"\"" || q.Peek(2) == "``")
                {
                    Dequeue(2);
                    return new KeyValuePair<string, string>(name, string.Empty);
                }

                if (q.Any())
                {
                    // attr=value is valid so check for the scenerio
                    if (q.Peek() == '\'' || q.Peek() == '"' || q.Peek() == '`')
                    {
                        var del = Dequeue();
                        var value = GetUpTo(del);
                        Dequeue();
                        return new KeyValuePair<string, string>(name, value);
                    }
                    else
                    {
                        var value = GetUpTo(' ', '>', '<', '\'', '"', '=', '`');
                        return new KeyValuePair<string, string>(name, value);
                    }
                }

                return new KeyValuePair<string, string>(name, null);
            }
        }

        private string GetComment()
        {
            this.Dequeue(4);
            var text = GetUpTo("-->");
            this.Dequeue(3);
            return text;
        }

        private string GetTagName() => GetUpTo(' ', '>', '/', '\r', '\n');

        private string GetUpTo(params char[] chars) => GetUpTo(() => chars.Contains(q.Peek()));

        private string GetUpTo(string text) => GetUpTo(() => this.q.Peek(text.Length) == text);

        private string GetUpTo(Func<bool> fn)
        {
            var text = string.Empty;
            while (q.Any())
            {
                text += Dequeue();
                if (!q.Any()) return text;
                if (fn())
                    break;
            }
            return text;
        }

        private IEnumerable<HtmlTag> InternalParse(HtmlTag parent = null)
        {
            var list = new List<HtmlTag>();
            HtmlTag node = null;
            while (q.Any())
            {
                var c = q.Peek();

                if (q.Peek(2) == "</")
                {
                    Dequeue(2);
                    GetUpTo('>');
                    Dequeue();
                    return list;
                }
                else if (q.Peek(4) == "<!--")
                {
                    var comment = GetComment();
                    node = new HtmlTag(HtmlNodeType.Comment) { };
                    node.Content = comment;
                    list.Add(node);
                }
                else if (c == '<')
                {
                    node = new HtmlTag();

                    Dequeue();
                    node.TagName = GetTagName();

                    if (q.Peek() == ' ' || q.Peek() == '\r')

                        node.AddAttributes(GetAttributes());

                    SkipSpace();

                    if (!q.Any())
                    {
                        list.Add(node);
                        break;
                    }

                    // instantly closed
                    if (q.Peek() == '>' && (q.Peek(3) == "></"))
                    {
                        Dequeue(3 + node.TagName.Length);

                        Dequeue();

                        list.Add(node);
                    }
                    // self closing element
                    else if (q.Peek(2) == "/>")
                    {
                        Dequeue(2);
                        node.SelfClosing = true;

                        list.Add(node);
                    }
                    // self closing tags that don't have '/>' ie: <br>
                    else if (q.Peek() == '>' && SelfClosingTags.Contains(node.TagName))
                    {
                        Dequeue();
                        node.SelfClosing = true;
                        list.Add(node);
                    }
                    else if (node.TagName.Equals("script", StringComparison.OrdinalIgnoreCase))
                    {
                        Dequeue(); // >
                        node._content = GetUpTo("</script");

                        Dequeue(9);
                        list.Add(node);
                    }
                }
                else if (c == '>')
                {
                    Dequeue();
                    node.AddChildren(InternalParse(node));

                    list.Add(node);
                }
                else
                {
                    var text = string.Empty;

                    while (q.Any())
                    {
                        text += Dequeue();
                        if (q.Any() && q.Peek() == '<') break;
                    }
                    if (text.IsNotBlank()) list.Add(new HtmlTag(HtmlNodeType.Text) { Content = text });
                }
            }

            return list;
        }

        private void SkipSpace()
        {
            while (q.Any() && (q.Peek() == ' ' || q.Peek() == '\r' || q.Peek() == '\n'))
                Dequeue();
        }

        internal static HtmlParser Instance = new HtmlParser();
        public IEnumerable<string> SelfClosingTags { get; set; } = new[] { "area", "base", "br", "col", "embed", "hr", "img", "input", "keygen", "link", "menuitem", "meta", "param", "source", "track", "wbr" };

        public IEnumerable<HtmlTag> Parse(string source)
        {
            if (source.IsNotBlank())
            {
                this.q = new Queue<char>(source);

                return InternalParse();
            }
            return Array.Empty<HtmlTag>();
        }
    }

    public enum HtmlNodeType
    {
        Element,
        Comment,
        Text
    }
}