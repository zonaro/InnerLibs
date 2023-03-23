using System;
using System.Collections.Generic;

namespace Extensions.Web.Selectors
{
    internal class TagNameSelector : CssSelector
    {
        public override string Token => string.Empty;

        protected internal override IEnumerable<HtmlNode> FilterCore(IEnumerable<HtmlNode> currentNodes)
        {
            foreach (var node in currentNodes)
            {
                if (node.TagName.Equals(this.Selector, StringComparison.InvariantCultureIgnoreCase))
                    yield return node;
            }
        }
    }
}
