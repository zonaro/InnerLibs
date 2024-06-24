using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Extensions.Web
{
    /// <summary>
    /// Holds the nodes of a parsed HTML or XML document. Use the <see cref="HtmlElementNode.ChildNodes"/>
    /// property to access these nodes. Use the <see cref="ToString()"/> method to convert the nodes
    /// back to markup.
    /// </summary>
    public class HtmlDocument : HtmlElementNode
    {
        /// <inheritdoc/>
        public override string OuterHtml => $"{this.HeaderNode?.ToString()}{base.OuterHtml}";

        /// <inheritdoc/>
        public override string TagName { get => "html"; set => base.TagName = value.IfBlank("html"); }

        /// <summary>
        /// Current document BODY tag. Return this <see cref="HtmlDocument"/> if body is not present
        /// </summary>
        public HtmlElementNode Body => ChildNodes.FirstOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("body")) ?? this;

        /// <summary>
        /// Document Meta Charset
        /// </summary>
        public string Charset
        {
            get => (this.ChildNodes.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.HasAttribute("charset"))?.GetAttribute("charset")).IfBlank(Encoding?.HeaderName);
            set
            {
                if (value.IsValid())
                {
                    var m = this.ChildNodes.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.HasAttribute("charset")) ?? new HtmlElementNode("meta");
                    m.SetAttribute("charset", value);
                    Head?.Add(m);

                    Util.TryExecute(() => this.Encoding = Encoding.GetEncoding(value));

                    this.Encoding = this.Encoding ?? new UTF8Encoding(false);
                }
            }
        }

        /// <summary>
        /// The current HEAD tag of document. Return BODY tag if HEAD is not present
        /// </summary>
        public HtmlElementNode Head => ChildNodes.FirstOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("head")) ?? Body;

        /// <summary>
        /// Document language
        /// </summary>
        public string Language
        {
            get => this.GetAttribute("lang");
            set
            {
                if (value.IsValid())
                {
                    this.SetAttribute("lang", value);
                }
                else
                {
                    this.RemoveAttribute("lang");
                }
            }
        }
        /// <summary>
        /// Document Meta Author
        /// </summary>
        public string Author
        {
            get => GetMeta(nameof(Author));
            set => SetMeta(nameof(Author), value);
        }

        /// <summary>
        /// Document Meta Description 
        /// </summary>
        public string Description
        {
            get => GetMeta(nameof(Description));
            set => SetMeta(nameof(Description), value);
        }

        /// <summary>
        /// Document title
        /// </summary>
        public string Title
        {
            get => this.FindFirst("title")?.InnerHtml;
            set
            {
                if (value.IsValid())
                {
                    var m = this.FindFirst("title") ?? new HtmlElementNode("title");
                    m.InnerHtml = value;
                    Head.Add(m);
                }
            }
        }

        /// <summary>
        /// Add inline CSS to HEAD
        /// </summary>
        /// <param name="InnerCss"></param>
        /// <returns></returns>
        public HtmlElementNode AddInlineCss(string InnerCss)
        {
            if (InnerCss.IsValid())
            {
                var stl = new HtmlElementNode("style");
                stl.AddText(InnerCss);
                Head.Add(stl);
                return stl;
            }
            return null;
        }

        /// <summary>
        /// Add inline Javascript tag to Body
        /// </summary>
        /// <param name="jsString"></param>
        /// <returns></returns>
        public HtmlElementNode AddInlineScript(string jsString)
        {
            if (jsString.IsValid())
            {
                var stl = new HtmlElementNode("script");
                stl.AddText(jsString);
                Body.Add(stl);
                return stl;
            }
            return null;
        }

        /// <summary>
        /// Add a JavaScript file to HEAD or BODY tag
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public HtmlElementNode AddScript(string src, bool AddToHead = false)
        {
            if (src.IsValid())
            {
                var scripto = new HtmlElementNode("script", new { src });
                if (AddToHead) Head.Add(scripto); else Body.Add(scripto);
                return scripto;
            }
            return null;
        }

        /// <summary>
        /// Add a CSS stylesheet link into HEAD tag
        /// </summary>
        /// <param name="href"></param>
        /// <returns></returns>
        public HtmlElementNode AddStyleSheet(string href)
        {
            if (href.IsValid())
            {
                var sheet = new HtmlElementNode("link", new { rel = "stylesheet", href });
                Head.Add(sheet);
                return sheet;
            }
            return null;
        }

        /// <inheritdoc/>
        public override string ToString() => OuterHtml;




        /// <summary>
        /// Save the current <see cref="HtmlDocument"/> into a file and return a <see cref="FileInfo"/>
        /// </summary>
        /// <returns></returns>
        public FileInfo Save() => SaveAs(this.File);

        /// <summary>
        /// Save the current <see cref="HtmlDocument"/> into a file and return a <see cref="FileInfo"/>
        /// </summary>
        /// <returns></returns>

        public FileInfo SaveAs(string file)
        {
            if (file.IsValid() && file.IsFilePath())
            {
                return SaveAs(file.ToFileInfo());
            }
            else
            {
                throw new ArgumentNullException(nameof(file), "File is not a valid file path");
            }
        }
        public FileInfo SaveAs(DirectoryInfo directory)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory), "Directory is null");
            }

            return SaveAs($"{directory.FullName}{Path.DirectorySeparatorChar}{this.Title.IfBlank($"{DateTime.Now.Ticks}")}.html");
        }

        public FileInfo SaveAs(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File is null");
            }

            this.File = ToString().WriteToFile(file, false, this.Encoding);
            return this.File;
        }

        public HtmlElementNode SetMeta(string name, string content)
        {
            if (name.IsValid())
            {
                var m = this.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.GetAttribute("name") == name) ?? new HtmlElementNode("meta");
                m.SetAttribute("name", name);
                m.SetAttribute("content", content);
                Head.Add(m);
                return m;
            }
            return null;
        }

        public HtmlElementNode GetMeta(string name) => name.IsValid() ? this.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.GetAttribute("name") == name) : null;

        /// <summary>
        /// Gets the source document path. May be empty or <c>null</c> if there was no source file.
        /// </summary>
        public FileInfo File { get; set; }

        /// <summary>
        /// Document encoding
        /// </summary>
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
            this.Encoding = new UTF8Encoding(false);
        }

        /// <summary>
        /// Return a default HTML template
        /// </summary>
        public const string DefaultTemplate = "<!DOCTYPE html ><html lang=\"en\"><head>  <meta charset=\"utf-8\">  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">  <title></title>  <meta name=\"description\" content=\"\">  <meta name=\"author\" content=\"\">  <meta property=\"og:title\" content=\"\">  <meta property=\"og:type\" content=\"website\">  <meta property=\"og:url\" content=\"\">  <meta property=\"og:description\" content=\"\">  <meta property=\"og:image\" content=\"\">  <link rel=\"icon\" href=\"/favicon.ico\"> <link rel=\"apple-touch-icon\" href=\"\"></head><body></body></html>";


        /// <summary>
        /// Create a <see cref="HtmlDocument"/> using the <see cref="HtmlDocument.DefaultTemplate"/>
        /// </summary>
        /// <returns></returns>
        public static HtmlDocument CreateDefault() => new HtmlDocument(DefaultTemplate);

        public HtmlDocument(FileInfo file, Encoding encoding = null) : this(file?.ReadAllText(encoding))
        {
            this.File = file;
            this.Encoding = encoding ?? new UTF8Encoding(false);
            this.Charset = this.Encoding.HeaderName;
        }

        private void buildHtml(string HtmlString)
        {

            this.Clear();

            var n = new HtmlParser().ParseChildren(HtmlString).ToList();

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

            this.Charset = $"{Charset}"; //move o charset pro lugar certo
        }

        public HtmlDocument(string HtmlString) : this() => buildHtml(HtmlString);

        public HeaderNode HeaderNode { get; set; }

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
        public T FirstOfType<T>(Func<T, bool> predicate) where T : HtmlNode => FindOfType(predicate).FirstOrDefault();
    }
}