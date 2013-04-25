using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using CsQuery.Implementation;

namespace CsQuery
{
    /// <summary>
    /// Interface to a a read-only, strongly-typed node list.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// Generic type parameter.
    /// </typeparam>

    public interface INodeList<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T> where T : IDomObject
    {
        /// <summary>
        /// The number of nodes in this INodeList
        /// </summary>

        int Length { get; }

        /// <summary>
        /// Get the item at the specified index
        /// </summary>
        ///
        /// <param name="index">
        /// Zero-based index of the item
        /// </param>
        ///
        /// <returns>
        /// An item
        /// </returns>

        T Item(int index);

        /// <summary>
        /// Converts this object to a read-only list.
        /// </summary>
        ///
        /// <returns>
        /// This object as an IList&lt;IDomObject&gt;
        /// </returns>

        IList<T> ToList();

    }
}
