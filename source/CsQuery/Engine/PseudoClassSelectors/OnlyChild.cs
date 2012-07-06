using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Mathches elements that are the the first child of a parent
    /// </summary>
    ///
    /// <url>
    /// http://reference.sitepoint.com/css/pseudoclass-firstchild
    /// </url>

    public class OnlyChild : PseudoSelectorElement
    {
        public override bool Matches(IDomElement element)
        {
            return OnlyChildOrNull(element.ParentNode) == element;
        }

        public override IEnumerable<IDomObject> ChildMatches(IDomElement element)
        {
            IDomObject child = OnlyChildOrNull(element);
            if (child != null)
            {
                yield return child;
            }
        }

        protected IDomObject OnlyChildOrNull(IDomObject parent)
        {
            if (parent.NodeType == NodeType.DOCUMENT_NODE)
            {
                return null;
            }
            else
            {
                return parent.ChildElements.SingleOrDefaultAlways();
            }
            
        }
    }
}
