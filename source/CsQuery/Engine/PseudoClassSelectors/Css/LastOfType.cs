using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Matches the last element of the same type within its siblings
    /// </summary>
    ///
    /// <url>
    /// http://reference.sitepoint.com/css/pseudoclass-lastoftype
    /// </url>

    public class LastOfType : PseudoSelectorChild
    {
        public override bool Matches(IDomObject element)
        {

            return element.ParentNode.ChildElements
              .Where(item => item.NodeNameID == element.NodeNameID)
              .LastOrDefault() == element;
        }

        public override IEnumerable<IDomObject> ChildMatches(IDomContainer element)
        {
            HashSet<ushort> Types = new HashSet<ushort>();
            foreach (var child in element.ChildElements.Reverse())
            {
                if (!Types.Contains(child.NodeNameID))
                {
                    Types.Add(child.NodeNameID);
                    yield return child;
                }
            }
        }

    }
}
