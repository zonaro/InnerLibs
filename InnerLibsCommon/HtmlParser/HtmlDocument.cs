using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Extensions.Web
{
    /// <summary>
    /// Holds the nodes of a parsed HTML or XML document. Use the <see cref="this.ChildNodes"/>
    /// property to access these nodes. Use the <see cref="ToString()"/> method to convert the nodes
    /// back to markup.
    /// </summary>
    public class HtmlDocument : HtmlElementNode
    {
        public override string OuterHtml => $"{this.HeaderNode?.ToString()}{base.OuterHtml}";
        public override string TagName { get => "html"; set => base.TagName = value.IfBlank("html"); }
        public HtmlElementNode Body => ChildNodes.FirstOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("body")) ?? HtmlRoot;

        public string Charset
        {
            get => (this.ChildNodes.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.HasAttribute("charset"))?.GetAttribute("charset")).IfBlank(Encoding?.HeaderName);
            set
            {
                if (value.IsNotBlank())
                {
                    var m = this.ChildNodes.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.HasAttribute("charset")) ?? new HtmlElementNode("meta");
                    m.SetAttribute("charset", value);
                    Head?.Add(m);

                    Util.TryExecute(() => this.Encoding = Encoding.GetEncoding(value));

                    this.Encoding = this.Encoding ?? new UTF8Encoding();
                }
            }
        }

        public HtmlElementNode Head => ChildNodes.FirstOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("head")) ?? Body;
        public HtmlElementNode HtmlRoot => ChildNodes.FirstOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("html")) ?? this;

        public string Language
        {
            get => HtmlRoot.GetAttribute("lang");
            set
            {
                if (value.IsNotBlank())
                {
                    HtmlRoot.SetAttribute("lang", value);
                }
            }
        }

        public string Author
        {
            get => GetMeta(nameof(Author));
            set => SetMeta(nameof(Author), value);
        }

        public string Description
        {
            get => GetMeta(nameof(Description));
            set => SetMeta(nameof(Description), value);
        }

        public string Title
        {
            get => HtmlRoot.FindFirst("title")?.InnerHtml;
            set
            {
                if (value.IsNotBlank())
                {
                    var m = HtmlRoot.FindFirst("title") ?? new HtmlElementNode("title");
                    m.InnerHtml = value;
                    Head.Add(m);
                }
            }
        }

        public HtmlElementNode AddInlineCss(string InnerCss)
        {
            if (InnerCss.IsNotBlank())
            {
                var stl = new HtmlElementNode("style");
                stl.AddText(InnerCss);
                Head.Add(stl);
                return stl;
            }
            return null;
        }

        public HtmlElementNode AddInlineScript(string jsString)
        {
            if (jsString.IsNotBlank())
            {
                var stl = new HtmlElementNode("script");
                stl.AddText(jsString);
                Body.Add(stl);
                return stl;
            }
            return null;
        }

        public HtmlElementNode AddScript(string src)
        {
            if (src.IsNotBlank())
            {
                var scripto = new HtmlElementNode("script", new { src });
                Body.Add(scripto);
                return scripto;
            }
            return null;
        }

        public HtmlElementNode AddStyle(string href)
        {
            if (href.IsNotBlank())
            {
                var sheet = new HtmlElementNode("link", new { rel = "stylesheet", href });
                Head.Add(sheet);
                return sheet;
            }
            return null;
        }

        public override string ToString() => OuterHtml;

        public FileInfo Save() => this.ToString().WriteToFile(File, false, this.Encoding);

        public HtmlElementNode SetMeta(string name, string content)
        {
            if (name.IsNotBlank())
            {
                var m = this.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.GetAttribute("name") == name) ?? new HtmlElementNode("meta");
                m.SetAttribute("name", name);
                m.SetAttribute("content", content);
                Head.Add(m);
                return m;
            }
            return null;
        }

        public HtmlElementNode GetMeta(string name)
        {
            if (name.IsNotBlank())
            {
                return this.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.GetAttribute("name") == name);

            }
            return null;
        }

        /// <summary>
        /// Gets the source document path. May be empty or <c>null</c> if there was no source file.
        /// </summary>
        public FileInfo File { get; set; }

        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets whether the library enforces HTML rules when parsing markup. This setting
        /// is global for all instances of this class.
        /// </summary>
        public static bool IgnoreHtmlRules
        {
            get => HtmlRules.IgnoreHtmlRules;
            set => HtmlRules.IgnoreHtmlRules = value;
        }

        /// <summary> Initializes an empty <see cref="HtmlDocument"> instance. </summary>
        public HtmlDocument() : base()
        {
            this.TagName = "html";
            this.Encoding = new UTF8Encoding();
        }

        public HtmlDocument(FileInfo file, Encoding encoding = null) : this(file?.ReadAllText(encoding))
        {
            this.File = file;
            this.Encoding = encoding ?? new UTF8Encoding();
            this.Charset = this.Encoding.HeaderName;
        }

        public HtmlDocument(string HtmlString) : this()
        {
            var n = new HtmlParser().ParseChildren(HtmlString);

            this.Add(n);

            var head = n.FindOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("head"));

            this.Add(head);

            var body = n.FindOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("body"));

            this.Add(body);

            var title = n.FindOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("title"));

            Head.Add(title);

            var meta = n.FindOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("meta"));

            Head.Add(meta);

            var hh = n.FindOfType<HtmlHeaderNode>();
            HtmlNode item = null;
            do
            {
                item = hh?.FirstOrDefault();
                HeaderNode = item?.Detach() as HtmlHeaderNode ?? HeaderNode;
                hh = hh?.Where(x => x != item);
            } while (item != null);

            var xh = n.FindOfType<XmlHeaderNode>();
            do
            {
                item = xh?.FirstOrDefault();
                HeaderNode = item?.Detach() as XmlHeaderNode ?? HeaderNode;
                xh = xh?.Where(x => x != item);
            } while (item != null);

            var html = n.FindOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("html"));
            foreach (var x in html)
            {
                foreach (var att in x.Attributes.AsEnumerable())
                {
                    this.SetAttribute(att.Key, att.Value);
                }
                this.Add(x.ChildNodes);
            }

            var outros = this.Select(x => x as HtmlElementNode)
                   .WhereNotNull()
                   .Where(x => x.TagName.EqualsIgnoreCaseAndAccents("html") && x.Any() == false).ToList();

            foreach (var o in outros)
            {
                o.Detach();
            }
        }

        public HtmlElementNode HeaderNode { get; private set; }

        /// <summary>
        /// Recursively searches this document's nodes for ones matching the specified selector.
        /// </summary>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> QuerySelectorAll(string selector) => ChildNodes.QuerySelectorAll(selector);

        /// <summary>
        /// Recursively searches this document's nodes for ones matching the specified compiled selectors.
        /// </summary>
        /// <param name="selectors">Compiled selectors that describe the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> QuerySelectorAll(SelectorCollection selectors) => ChildNodes.QuerySelectorAll(selectors);

        /// <summary>
        /// Recursively finds all HtmlNodes in this document for which the given predicate returns true.
        /// </summary>
        /// <param name="predicate">
        /// A function that determines if the item should be included in the results.
        /// </param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlNode> QuerySelectorAll(Func<HtmlNode, bool> predicate) => ChildNodes.QuerySelectorAll(predicate);

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>() where T : HtmlNode => ChildNodes.FindOfType<T>();

        /// <summary>
        /// Recursively finds all nodes of the specified type, and for which the given predicate
        /// returns true.
        /// </summary>
        /// <param name="predicate">
        /// A function that determines if the item should be included in the results.
        /// </param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>(Func<T, bool> predicate) where T : HtmlNode => ChildNodes.FindOfType(predicate);
        public T FirstOfType<T>(Func<T, bool> predicate) where T : HtmlNode => FindOfType<T>(predicate).FirstOrDefault();
    }
}