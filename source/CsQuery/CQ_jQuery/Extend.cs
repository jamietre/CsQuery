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
        /// Map each property of the objects in sources to the target object.  Returns an expando object
        /// (either the target object, if it's an expando object, or a new expando object)
        /// </summary>
        ///
        /// <param name="target">
        /// .
        /// </param>
        /// <param name="sources">
        /// .
        /// </param>
        ///
        /// <returns>
        /// .
        /// </returns>

        public static object Extend(object target, params object[] sources)
        {
            return Objects.Extend(false, target, sources);
        }

        /// <summary>
        /// Map each property of the objects in sources to the target object.  Returns an expando object
        /// (either the target object, if it's an expando object, or a new expando object)
        /// </summary>
        ///
        /// <param name="deep">
        /// true to deep.
        /// </param>
        /// <param name="target">
        /// .
        /// </param>
        /// <param name="sources">
        /// .
        /// </param>
        ///
        /// <returns>
        /// .
        /// </returns>

        public static object Extend(bool deep, object target, params object[] sources)
        {
            return Objects.Extend(deep, target, sources);
        }
    }
}
