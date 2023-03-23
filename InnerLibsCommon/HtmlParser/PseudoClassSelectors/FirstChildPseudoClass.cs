namespace Extensions.Web.PseudoClassSelectors
{
    [PseudoClassName("first-child")]
    internal class FirstChildPseudoClass : PseudoClass
    {
        protected override bool CheckNode(HtmlNode node, string parameter) => node.Index == 0;
    }
}