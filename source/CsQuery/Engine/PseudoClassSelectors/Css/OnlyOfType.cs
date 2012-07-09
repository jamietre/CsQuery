using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Mathches elements that are the only sibling of this type.
    /// </summary>
    ///
    /// <url>
    /// http://reference.sitepoint.com/css/pseudoclass-onlyoftype
    /// </url>

    public class OnlyOfType: PseudoSelectorChild
    {
        public override bool Matches(IDomObject element)
        {

            return element.ParentNode.ChildElements
               .Where(item => item.NodeNameID == element.NodeNameID)
               .SingleOrDefaultAlways() != null;
        }

        public override IEnumerable<IDomObject> ChildMatches(IDomContainer element)
        {
            return OnlyChildOfAnyType(element);
        }

        /// <summary>
        /// When there's no type, it must return all children that are the only one of that type
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private IEnumerable<IDomObject> OnlyChildOfAnyType(IDomObject parent)
        {
            IDictionary<ushort, IDomElement> Types = new Dictionary<ushort, IDomElement>();
            foreach (var child in parent.ChildElements)
            {
                if (Types.ContainsKey(child.NodeNameID))
                {
                    Types[child.NodeNameID] = null;
                }
                else
                {
                    Types[child.NodeNameID] = child;
                }
            }
            // if the value is null, there was more than one of the type
            return Types.Values.Where(item => item != null);
        }


    }
}
