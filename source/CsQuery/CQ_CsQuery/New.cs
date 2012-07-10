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
        /// Create a new, empty CsQuery object bound to this domain.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>

        public CQ New()
        {
            CQ csq = new CQ();
            csq.CsQueryParent = this;
            return csq;
        }
    }
}
