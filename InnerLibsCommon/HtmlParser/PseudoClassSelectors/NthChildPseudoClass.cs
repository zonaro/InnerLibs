namespace Extensions.Web.PseudoClassSelectors
{
    [PseudoClassName("nth-child")]
    internal class NthChildPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.Index == int.Parse(parameter) - 1;
    }
}