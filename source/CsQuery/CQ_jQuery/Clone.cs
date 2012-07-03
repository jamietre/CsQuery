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
        /// Create a deep copy of the set of matched elements.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object that contains a clone of each element in the original selection set.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/clone/
        /// </url>

        public CQ Clone()
        {
            CQ csq = new CQ();

            // TODO: The type of document needs to be implemented as a factory. THere are certainly other places
            // where this choice should be made.

            if (Document is IDomFragment)
            {
                csq.CreateNewFragment();
            }
            else
            {
                csq.CreateNewDocument();
            }

            foreach (IDomObject elm in SelectionSet)
            {
                IDomObject clone = elm.Clone();
                csq.Document.ChildNodes.AddAlways(clone);
                csq.AddSelection(clone);
            }
            return csq;
        }
        

    }
}
