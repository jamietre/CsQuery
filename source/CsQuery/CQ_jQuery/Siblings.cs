using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;
using CsQuery.Implementation;

namespace CsQuery
{
    public partial class CQ
    {
        /// <summary>
        /// Description: Get the siblings of each element in the set of matched elements, optionally
        /// filtered by a selector.
        /// </summary>
        ///
        /// <param name="selector">
        /// A selector used to filter the siblings.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/siblings/
        /// </url>

        public CQ Siblings(string selector = null)
        {
            SelectionSet<IDomElement> siblings = new SelectionSet<IDomElement>();

            // Add siblings of each item in the selection except the item itself for that iteration.
            // If two siblings are in the selection set, then all children of their mutual parent should
            // be returned. Otherwise, all children except the item iteself.
            foreach (var item in SelectionSet)
            {
                foreach (var child in item.ParentNode.ChildElements)
                {
                    if (!ReferenceEquals(child, item))
                    {
                        siblings.Add(child);
                    }
                }
            }
            return FilterIfSelector(selector, siblings, SelectionSetOrder.Ascending);
        }

    }
}
