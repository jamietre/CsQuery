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
    public interface IDomIndex 
    {

        /// <summary>
        /// The primary selection index.
        /// </summary>

        RangeSortedDictionary<ushort,IDomObject> SelectorXref { get; }

        /// <summary>
        /// Adds an element to the index.
        /// </summary>
        ///
        /// <param name="key">
        /// The index key. This should be a unique path to the element in the Document tree. The format
        /// is determined by environmental settings. This is for internal use.
        /// </param>
        /// <param name="element">
        /// The element.
        /// </param>

        void AddToIndex(ushort[] key, IDomIndexedNode element);

        /// <summary>
        /// Adds an element to the index.
        /// </summary>
        ///
        /// <param name="element">
        /// The element.
        /// </param>

        void AddToIndex(IDomIndexedNode element);

        /// <summary>
        /// Removes an element from the index
        /// </summary>
        ///
        /// <param name="key">
        /// The index key. This should be a unique path to the element in the Document tree. The format
        /// is determined by environmental settings. This is for internal use.
        /// </param>

        void RemoveFromIndex(ushort[] key);

        /// <summary>
        /// Removes an element from the index.
        /// </summary>
        ///
        /// <param name="element">
        /// The element.
        /// </param>

        void RemoveFromIndex(IDomIndexedNode element);

        /// <summary>
        /// Queries the index
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

        /// <summary>
        /// Queries the index.
        /// </summary>
        ///
        /// <param name="subKey">
        /// The sub key.
        /// </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process query index in this collection.
        /// </returns>

        IEnumerable<IDomObject> QueryIndex(ushort[] subKey);

        /// <summary>
        /// Any user data to be persisted with this DOM.
        /// </summary>

        IDictionary<string, object> Data { get; set; }
    }
}
