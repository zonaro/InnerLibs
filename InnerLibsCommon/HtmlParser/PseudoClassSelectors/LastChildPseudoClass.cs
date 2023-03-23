using System.Linq;

namespace Extensions.Web.PseudoClassSelectors
{
    [PseudoClassName("last-child")]
    internal class LastChildPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.ParentNode.GetChildElements().Last() == node;
    }
}