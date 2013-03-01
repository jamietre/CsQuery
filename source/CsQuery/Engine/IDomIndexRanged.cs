using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Implementation;

namespace CsQuery
{
    /// <summary>
    /// Interface for a DOM index. Defines methods to add and remove items from the index, and query the index.
    /// </summary>
    public interface IDomIndexRanged: IDomIndex
    {
        /// <summary>
        /// When true, changes are queued until the next read operation
        /// </summary>

        bool QueueChanges { get; set; }


        /// <summary>
        /// Queries the index, returning all matching elements
        /// </summary>
        ///
        /// <param name="subKey">
        /// The sub key.
        /// </param>
        /// <param name="depth">
        /// The depth.
        /// </param>
        /// <param name="includeDescendants">
        /// true to include, false to exclude the descendants.
        /// </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process query index in this collection.
        /// </returns>

        IEnumerable<IDomObject> QueryIndex(ushort[] subKey, int depth, bool includeDescendants);

    }
}
