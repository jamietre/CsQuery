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

    public class FirstChild : PseudoSelectorChild
    {
        public override bool Matches(IDomObject element)
        {

            return element.ParentNode.FirstElementChild == element;
        }

        public override IEnumerable<IDomObject> ChildMatches(IDomContainer element)
        {
            IDomObject child = element.FirstChild;
            if (child != null)
            {
                yield return element.FirstElementChild;
            }
        }

    }
}
