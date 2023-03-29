// Copyright (c) 2019-2022 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Extensions.Web
{


    /// <summary>
    /// Holds the nodes of a parsed HTML or XML document. Use the <see cref="RootNodes"/>
    /// property to access these nodes. Use the <see cref="ToHtml"/> method to convert the
    /// nodes back to markup.
    /// </summary>
    public class HtmlDocument
    {

        public HtmlElementNode Body => RootNodes.FirstOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("body")) ?? HtmlRoot;

        public string Charset
        {
            get => this.RootNodes.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.HasAttribute("charset"))?.GetAttribute("charset");
            set
            {
                if (value.IsNotBlank())
                {
                    var m = this.RootNodes.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.HasAttribute("charset")) ?? new HtmlElementNode("meta");
                    m.SetAttribute("charset", value);
                    Head?.AddChildren(m);
                }
            }
        }

        public HtmlElementNode Head => RootNodes.FirstOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("head")) ?? Body;
        public HtmlElementNode HtmlRoot => RootNodes.FirstOfType<HtmlElementNode>(x => x.TagName.EqualsIgnoreCaseAndAccents("html"));

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

        public string Title
        {
            get => HtmlRoot.FindFirst("title")?.InnerHtml;
            set
            {
                if (value.IsNotBlank())
                {
                    var m = HtmlRoot.FindFirst("title") ?? new HtmlElementNode("title");
                    m.InnerHtml = value;
                    Head.AddChildren(m);
                }
            }
        }


        public HtmlElementNode AddInlineCss(string InnerCss)
        {
            if (InnerCss.IsNotBlank())
            {
                var stl = new HtmlElementNode("style");
                stl.AddText(InnerCss);
                Head.AddChildren(stl);
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
                Body.AddChildren(stl);
                return stl;
            }
            return null;
        }

        public HtmlElementNode AddScript(string src)
        {
            if (src.IsNotBlank())
            {
                var scripto = new HtmlElementNode("script", new { src });
                Body.AddChildren(scripto);
                return scripto;
            }
            return null;
        }

        public HtmlNode AddStyle(string href)
        {
            if (href.IsNotBlank())
            {
                var sheet = new HtmlElementNode("link", new { rel = "stylesheet", href });
                Head.AddChildren(sheet);
                return sheet;
            }
            return null;
        }

        public FileInfo Save(string filename) => this.ToString().WriteToFile(filename);

        public HtmlElementNode SetMeta(string name, string content)
        {
            if (name.IsNotBlank())
            {
                var m = this.FirstOfType<HtmlElementNode>(x => x.TagName == "meta" && x.GetAttribute("name") == name) ?? new HtmlElementNode("meta");
                m.SetAttribute("name", name);
                m.SetAttribute("content", content);
                Head.AddChildren(m);
                return m;
            }
            return null;
        }


        /// <summary>
        /// Gets the source document path. May be empty or <c>null</c> if there was no
        /// source file.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the document root nodes. Provides access to all document nodes.
        /// </summary>
        public HtmlNodeCollection RootNodes { get; private set; }

        /// <summary>
        /// Gets or sets whether the library enforces HTML rules when parsing markup.
        /// This setting is global for all instances of this class.
        /// </summary>
        public static bool IgnoreHtmlRules
        {
            get => HtmlRules.IgnoreHtmlRules;
            set => HtmlRules.IgnoreHtmlRules = value;
        }

        /// <summary>
        /// Initializes an empty <see cref="HtmlDocument"> instance.
        /// </summary>
        public HtmlDocument()
        {
            Path = null;
            RootNodes = new HtmlNodeCollection(null);
        }


        /// <summary>
        /// Recursively searches this document's nodes for ones matching the specified selector.
        /// </summary>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> QuerySelectorAll(string selector) => RootNodes.QuerySelectorAll(selector);

        /// <summary>
        /// Recursively searches this document's nodes for ones matching the specified compiled
        /// selectors.
        /// </summary>
        /// <param name="selectors">Compiled selectors that describe the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> QuerySelectorAll(SelectorCollection selectors) => RootNodes.QuerySelectorAll(selectors);

        /// <summary>
        /// Recursively finds all HtmlNodes in this document for which the given predicate returns true.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included
        /// in the results.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlNode> QuerySelectorAll(Func<HtmlNode, bool> predicate) => RootNodes.QuerySelectorAll(predicate);

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>() where T : HtmlNode => RootNodes.FindOfType<T>();

        /// <summary>
        /// Recursively finds all nodes of the specified type, and for which the given predicate returns true.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in
        /// the results.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>(Func<T, bool> predicate) where T : HtmlNode => RootNodes.FindOfType(predicate);
        public T FirstOfType<T>(Func<T, bool> predicate) where T : HtmlNode => FindOfType<T>(predicate).FirstOrDefault();

        /// <summary>
        /// Generates an HTML string from the contents of this <see cref="HtmlDocument"></see>.
        /// </summary>
        /// <returns>A string with the markup for this document.</returns>
        public string ToHtml() => RootNodes.ToHtml();

        #region Static methods

        /// <summary>
        /// Parses an HTML or XML string and returns an <see cref="HtmlDocument"></see> instance that
        /// contains the parsed nodes.
        /// </summary>
        /// <param name="html">The HTML or XML string to parse.</param>
        /// <returns>Returns an <see cref="HtmlDocument"></see> instance that contains the parsed
        /// nodes.</returns>
        public static HtmlDocument FromHtml(string html) => new HtmlParser().Parse(html);

        /// <summary>
        /// Parses an HTML or XML file and returns an <see cref="HtmlDocument"></see> instance that
        /// contains the parsed nodes.
        /// </summary>
        /// <param name="path">The HTML or XML file to parse.</param>
        /// <returns>Returns an <see cref="HtmlDocument"></see> instance that contains the parsed
        /// nodes.</returns>
        public static HtmlDocument FromFile(string path) => FromHtml(File.ReadAllText(path));


        /// <summary>
        /// Parses an HTML or XML file and returns an <see cref="HtmlDocument"></see> instance that
        /// contains the parsed nodes.
        /// </summary>
        /// <param name="path">The HTML or XML file to parse.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <returns>Returns an <see cref="HtmlDocument"></see> instance that contains the parsed
        /// nodes.</returns>
        public static HtmlDocument FromFile(string path, Encoding encoding) => FromHtml(File.ReadAllText(path, encoding));



        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <returns>The matching nodes.</returns>
        [Obsolete("This method is deprecated and will be removed in a future version. Use IEnumerable<HtmlNode>.FindOfType() extension methods instead.")]
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes) where T : HtmlNode => nodes.FindOfType<T>();

        /// <summary>
        /// Recursively finds all nodes of the specified type filtered by the given predicate.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        /// <returns>The matching nodes.</returns>
        [Obsolete("This method is deprecated and will be removed in a future version. Use IEnumerable<HtmlNode>.FindOfType() extension methods instead.")]
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes, Func<T, bool> predicate) where T : HtmlNode => nodes.FindOfType<T>(predicate);

        #endregion

    }
}
