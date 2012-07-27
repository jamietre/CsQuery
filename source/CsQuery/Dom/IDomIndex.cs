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

        RangeSortedDictionary<IDomObject> SelectorXref { get; }

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

        void AddToIndex(string key, IDomIndexedNode element);
        void AddToIndex(IDomIndexedNode element);
        void RemoveFromIndex(string key);
        void RemoveFromIndex(IDomIndexedNode element);
        IEnumerable<IDomObject> QueryIndex(string subKey, int depth, bool includeDescendants);
        IEnumerable<IDomObject> QueryIndex(string subKey);

        int TokenizeString(int startIndex, int length);
        string GetTokenizedString(int index);
        char[] SourceHtml { get; }

        /// <summary>
        /// Any user data to be persisted with this DOM.
        /// </summary>

        IDictionary<string, object> Data { get; set; }
    }
}
