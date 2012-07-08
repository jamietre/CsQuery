using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Mathches elements that are the the last child of a parent
    /// </summary>
    ///
    /// <url>
    /// http://reference.sitepoint.com/css/pseudoclass-lastchild
    /// </url>

    public class LastChild: PseudoSelectorElement
    {
        public override bool Matches(IDomElement element)
        {

            return element.ParentNode.LastElementChild == element;
        }

        public override IEnumerable<IDomObject> ChildMatches(IDomElement element)
        {
            IDomElement child = element.LastElementChild;
            if (child != null)
            {
                yield return child;
            }
        }

    }
}
