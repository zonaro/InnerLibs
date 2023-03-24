using System.Collections.Generic;
using System.Linq;

namespace Extensions.Web.PseudoClassSelectors
{

    internal class PseudoClassSelector : CssSelector
    {
        public override string Token => ":";

        protected internal override IEnumerable<HtmlNode> FilterCore(IEnumerable<HtmlNode> currentNodes)
        {
            string[] values = this.Selector.TrimEnd(')').Split(new[] { '(' }, 2);

            var pseudoClass = PseudoClass.GetPseudoClass(values[0]);
            string value = values.Length > 1 ? values[1] : null;

            return pseudoClass.Filter(currentNodes, value);
        }
    }

    [PseudoClassName("gt")]
    internal class GtPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.Index > parameter.ToInt();
    }
    [PseudoClassName("lt")]
    internal class LtPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.Index < parameter.ToInt();
    }
    [PseudoClassName("first-child")]
    internal class FirstChildPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.Index == 0;
    }

    [PseudoClassName("last-child")]
    internal class LastChildPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.ParentNode.GetChildElements().Last() == node;
    }


    [PseudoClassName("nth-child")]
    internal class NthChildPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.Index == parameter.ToInt() - 1;
    }

    [PseudoClassName("contains")]
    internal class ContainsPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.Content.Contains(parameter);
    }

    [PseudoClassName("has")]
    internal class HasPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.QuerySelectorAll(parameter).Any();
    }


    [PseudoClassName("input")]
    internal class InputPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.TagName.IsIn("input", "textarea", "button");
    }

    [PseudoClassName("text")]
    internal class TextPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.TagName == "input" && node.GetAttribute("type") == "text";
    }

    [PseudoClassName("even")]
    internal class EvenPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.Index.IsEven();
    }

    [PseudoClassName("odd")]
    internal class OddPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.Index.IsOdd();
    }

    [PseudoClassName("not")]
    internal class NotPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter)
        {
            var selectors = CssSelector.Parse(parameter);
            var nodes = new[] { node };

            foreach (var selector in selectors)
                if (selector.FilterCore(nodes).Count() == 1)
                    return false;

            return true;
        }
    }
}