using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CsQuery
{
    /// <summary>
    /// Interface to a a read-only, strongly-typed node list.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// Generic type parameter.
    /// </typeparam>

    public interface INodeList<T>: IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T> where T: IDomObject
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

        /// <summary>
        /// Adds a sequence of elements to the end of this INodeList
        /// </summary>
        ///
        /// <param name="elements">
        /// An IEnumerable&lt;IDomObject&gt; of items to append to this.
        /// </param>

        //void AddRange(IEnumerable<IDomObject> elements);

        /// <summary>
        /// Add a node but do not attempt to clean up duplicate IDs or remove it from an existing DOM.
        /// This is required for the parser, but normally when you are using "Add" you want it to removed
        /// the ID from disconnected elements. This can also result in nodes appearing in more than one
        /// place in the DOM and should generally not be used by clients.
        /// 
        /// This may be used by end users, but should be used with caution.
        /// </summary>
        ///
        /// <param name="item">
        /// The object to add to this INodeList
        /// </param>

        //void AddAlways(IDomObject item);

        /// <summary>
        /// The element that owns this list
        /// </summary>
        ///
        /// <value>
        /// An IDomContainer element
        /// </value>

       //IDomContainer Owner { get; }
    }
}
