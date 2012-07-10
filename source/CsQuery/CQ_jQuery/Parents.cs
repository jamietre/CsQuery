using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;

namespace CsQuery
{
    public partial class CQ
    {
        #region public methods

        /// <summary>
        /// Get the parent of each element in the current set of matched elements, optionally filtered by
        /// a selector.
        /// </summary>
        ///
        /// <param name="selector">
        /// A string containing a selector expression to match elements against.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/parents/
        /// </url>

        public CQ Parent(string selector = null)
        {
            return FilterIfSelector(selector, MapRangeToNewCQ(Selection, parentImpl));
        }

        /// <summary>
        /// Get the ancestors of each element in the current set of matched elements, optionally filtered
        /// by a selector.
        /// </summary>
        ///
        /// <param name="filter">
        /// (optional) a selector which limits the elements returned.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/parents/
        /// </url>

        public CQ Parents(string filter = null)
        {
            return ParentsUntil((string)null, filter);
        }

        
        #endregion

        #region private methods

        protected IEnumerable<IDomObject> parentImpl(IDomObject input)
        {
            if (input.ParentNode != null &&
                input.ParentNode.NodeType == NodeType.ELEMENT_NODE)
            {
                yield return input.ParentNode;
            }
        }
        protected IEnumerable<IDomElement> parentsImpl(IEnumerable<IDomElement> source, HashSet<IDomElement> until)
        {

            HashSet<IDomElement> alreadyAdded = new HashSet<IDomElement>();

            foreach (var item in source)
            {
                int depth = item.Depth;
                IDomElement parent = item.ParentNode as IDomElement;
                while (parent != null && !until.Contains(parent))
                {
                    if (alreadyAdded.Add(parent))
                    {
                        yield return parent;
                    }
                    else
                    {
                        break;
                    }

                    parent = parent.ParentNode as IDomElement;
                }
            }


            //return results.Select(item => item.Item3);
            //var comp = new parentComparer();
            //return results.OrderBy(item=>item,comp).Select(item => item.Item3);
        }
        #endregion

        #region private classes

        class parentComparer : IComparer<Tuple<int, int, IDomElement>>
        {

            public int Compare(Tuple<int, int, IDomElement> x, Tuple<int, int, IDomElement> y)
            {
                int depth = y.Item1 - x.Item1;
                return depth != 0 ? depth : x.Item2 - y.Item2;
            }
        }
        #endregion
    }
}
