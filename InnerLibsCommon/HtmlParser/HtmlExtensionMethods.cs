// Copyright (c) 2019-2022 Jonathan Wood (www.softcircuits.com) Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.Web;

namespace Extensions
{
    public static partial class Util
    {
        #region Public Methods

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<T> FindOfType<T>(this IEnumerable<HtmlNode> nodes) where T : HtmlNode => QuerySelectorAll(nodes, n => n is T).Cast<T>();

        /// <summary>
        /// Recursively finds all nodes of the specified type for which the given predicate returns true.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="predicate">
        /// A function that determines if the item should be included in the results.
        /// </param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<T> FindOfType<T>(this IEnumerable<HtmlNode> nodes, Func<T, bool> predicate) where T : HtmlNode => QuerySelectorAll(nodes, n => n is T node && predicate(node)).Cast<T>();

        public static T FirstOfType<T>(this IEnumerable<HtmlNode> nodes, Func<T, bool> predicate) where T : HtmlNode => FindOfType(nodes, predicate).FirstOrDefault();

        public static HtmlNode QuerySelector(this HtmlElementNode tags, Func<HtmlNode, bool> query) => QuerySelector(tags.ChildNodes, query);

        public static HtmlNode QuerySelector(this IEnumerable<HtmlNode> tags, Func<HtmlNode, bool> query) => QuerySelectorAll(tags, query).FirstOrDefault();

        public static HtmlNode QuerySelector(this IEnumerable<HtmlNode> tags, string cssSelector) => QuerySelectorAll(tags, cssSelector).FirstOrDefault();

        public static HtmlNode QuerySelector(this HtmlNode node, string cssSelector) => QuerySelectorAll(node, cssSelector).FirstOrDefault();

        /// <summary>
        /// Recursively finds all nodes for which the given predicate returns true.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="predicate">
        /// A function that determines if the item should be included in the results.
        /// </param>
        /// <returns>The matching nodes.</returns>
        /// <remarks>Implemented without recursion for better performance on deeply nested collections.</remarks>
        public static IEnumerable<HtmlNode> QuerySelectorAll(this IEnumerable<HtmlNode> nodes, Func<HtmlNode, bool> predicate)
        {
            var stack = new Stack<IEnumerator<HtmlNode>>();
            var enumerator = nodes.GetEnumerator();

            try
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        HtmlNode node = enumerator.Current;
                        if (predicate(node))
                            yield return node;

                        if (node is HtmlElementNode elementNode)
                        {
                            stack.Push(enumerator);
                            enumerator = elementNode.ChildNodes.GetEnumerator();
                        }
                    }
                    else if (stack.Count > 0)
                    {
                        enumerator.Dispose();
                        enumerator = stack.Pop();
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
            finally
            {
                enumerator.Dispose();

                // Dispose enumerators in case of exception
                while (stack.Count > 0)
                {
                    enumerator = stack.Pop();
                    enumerator.Dispose();
                }
            }
        }


        public static IEnumerable<HtmlNode> QuerySelectorAll(this HtmlNode node, string cssSelector) => new[] { node }.QuerySelectorAll(cssSelector);

        /// <summary>
        /// Recursively searches the given nodes for ones matching the specified selectors.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<HtmlElementNode> QuerySelectorAll(this IEnumerable<HtmlNode> nodes, string selector)
        {
            SelectorCollection selectors = Selector.ParseSelector(selector);
            return selectors.Find(nodes);
        }

        /// <summary>
        /// Recursively searches the given nodes for ones matching the specified compiled selectors.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="selectors">Compiled selectors that describe the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<HtmlElementNode> QuerySelectorAll(this IEnumerable<HtmlNode> nodes, SelectorCollection selectors) => selectors.Find(nodes);

        /// <summary>
        /// Generates an HTML string from the contents of this node collection.
        /// </summary>
        /// <returns>A string with the markup for this node collection.</returns>
        public static string ToHtml(this IEnumerable<HtmlNode> nodes) => string.Concat(nodes.Select(n => n.OuterHtml));

        #endregion Public Methods
    }
}