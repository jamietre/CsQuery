using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Matches the first element of the same type within its siblings
    /// </summary>
    ///
    /// <url>
    /// http://reference.sitepoint.com/css/pseudoclass-firstoftype
    /// </url>

    public class FirstOfType : PseudoSelectorElement
    {
        public override bool Matches(IDomElement element)
        {

            return element.ParentNode.ChildElements
              .Where(item => item.NodeNameID == element.NodeNameID)
              .FirstOrDefault() == element;
        }

        public override IEnumerable<IDomObject> ChildMatches(IDomElement element)
        {
            HashSet<ushort> Types = new HashSet<ushort>();
            foreach (var child in element.ChildElements)
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
