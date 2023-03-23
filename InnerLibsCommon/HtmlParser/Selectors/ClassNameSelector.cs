using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions.Web.Selectors
{
    internal class ClassNameSelector : CssSelector
    {
        public override string Token => ".";

        protected internal override IEnumerable<HtmlNode> FilterCore(IEnumerable<HtmlNode> currentNodes)
        {
            foreach (var node in currentNodes)
            {
                if (node.ClassList.Any(c => c.Equals(this.Selector, StringComparison.InvariantCultureIgnoreCase)))
                    yield return node;
            }
        }
    }
}
