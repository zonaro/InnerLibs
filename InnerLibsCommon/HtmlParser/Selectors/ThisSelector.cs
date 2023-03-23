using System.Collections.Generic;

namespace Extensions.Web.Selectors
{

    internal class ThisSelector : CssSelector
    {
        public override string Token => "this";

        public override bool AllowTraverse => false;

        protected internal override IEnumerable<HtmlNode> FilterCore(IEnumerable<HtmlNode> currentNodes) => currentNodes;
    }
}