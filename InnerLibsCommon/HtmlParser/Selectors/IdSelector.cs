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
                if (node.ID.Equals(this.Selector, StringComparison.InvariantCultureIgnoreCase))
                    return new[] { node };
            }

            return new HtmlNode[0];
        }
    }
}
