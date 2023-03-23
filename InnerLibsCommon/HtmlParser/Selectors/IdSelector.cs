using System;
using System.Collections.Generic;

namespace Extensions.Web.Selectors
{
    internal class IdSelector : CssSelector
    {
        public override string Token => "#";

        protected internal override IEnumerable<HtmlNode> FilterCore(IEnumerable<HtmlNode> currentNodes)
        {
            foreach (var node in currentNodes)
            {
                if (node.Id.Equals(this.Selector, StringComparison.InvariantCultureIgnoreCase))
                    yield return node;
            }


        }
    }
}
