using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Implementation;
using CsQuery.HtmlParser;

namespace CsQuery.Engine
{
    /// <summary>
    /// Simple implementation of DOM index that only stores a reference to the index target. This
    /// will perform much better than the ranged index for dom construction &amp; manipulation, but
    /// worse for complex queries.
    /// </summary>

    public class DomIndexSimple: IDomIndex
    {
        /// <summary>
        /// Default constructor for the index
        /// </summary>

        public DomIndexSimple()
        {
            Index = new Dictionary<ushort[], ICollection<IDomObject>>(PathKeyComparer.Comparer);
        }

        private IDictionary<ushort[], ICollection<IDomObject>> Index;

        private ICollection<IDomObject> GetElementSet() {
            return new List<IDomObject>();
        }

        /// <summary>
        /// Add an element to the index using the default keys for this element.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to add
        /// </param>

        public void AddToIndex(IDomIndexedNode element)
        {

            if (element.HasChildren)
            {
                foreach (DomElement child in ((IDomContainer)element).ChildElements)
                {
                    AddToIndex(child);
                }
            }
            
            foreach (var key in element.IndexKeys())
            {
                AddToIndex(key, element);
            }
            
        }

         
        /// <summary>
        /// Adds an element to the index for the specified key.
        /// </summary>
        ///
        /// <param name="key">
        /// The key to remove.
        /// </param>
        /// <param name="element">
        /// The element to add.
        /// </param>

        public void AddToIndex(ushort[] key, IDomIndexedNode element)
        {
            ICollection<IDomObject> existing;
            if (!Index.TryGetValue(key, out existing))
            {
                existing = GetElementSet();
                existing.Add(element.IndexReference);
                Index.Add(key, existing);
            }
            else
            {
                existing.Add(element.IndexReference);
            }
        }

        /// <summary>
        /// Remove an element from the index using its key.
        /// </summary>
        ///
        /// <param name="key">
        /// The key to remove.
        /// </param>
        /// <param name="element">
        /// The element to remove.
        /// </param>

        public void RemoveFromIndex(ushort[] key, IDomIndexedNode element)
        {
            ICollection<IDomObject> existing;
            if (Index.TryGetValue(key, out existing))
            {
                existing.Remove(element.IndexReference);
            }
        }

        /// <summary>
        /// Remove an element from the index.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to remove
        /// </param>

        public void RemoveFromIndex(IDomIndexedNode element)
        {
            if (element.HasChildren)
            {
                foreach (IDomElement child in ((IDomContainer)element).ChildElements)
                {
                    if (child.IsIndexed)
                    {
                        RemoveFromIndex(child);
                    }
                }
            }

            foreach (ushort[] key in element.IndexKeys())
            {
                RemoveFromIndex(key,element);
            }
        }

        /// <summary>
        /// Query the document's index for a subkey.
        /// </summary>
        ///
        /// <param name="subKey">
        /// The subkey to match
        /// </param>
        ///
        /// <returns>
        /// A sequence of all matching keys
        /// </returns>

        public IEnumerable<IDomObject> QueryIndex(ushort[] subKey)
        {
            ICollection<IDomObject> existing;
            if (Index.TryGetValue(subKey, out existing))
            {
                return existing.OrderBy(item => item);

            }
            return Enumerable.Empty<IDomObject>();
        }

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>

        public void Clear()
        {
            Index.Clear();
        }

        /// <summary>
        /// The number of unique index keys.
        /// </summary>
        ///
        /// <returns>
        /// The count of items in the index.
        /// </returns>

        public int Count
        {
            get
            {
                return Index.Count;
            }
        }

        /// <summary>
        /// Returns the features that this index implements.
        /// </summary>

        public DomIndexFeatures Features
        {
            get { return DomIndexFeatures.Lookup; }
        }

        /// <summary>
        /// When true, changes are queued until the next read operation. For the SimpleIndex provider,
        /// this is always true; setting it has no effect.
        /// </summary>

        public bool QueueChanges
        {
            get
            {
                return false;
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// Queries the index, returning all matching elements. For the SimpleIndex, this is not implemented.
        /// </summary>
        ///
        /// <exception cref="NotImplementedException">
        /// Thrown when the requested operation is unimplemented.
        /// </exception>
        ///
        /// <param name="subKey">
        /// The subkey to match.
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

        public IEnumerable<IDomObject> QueryIndex(ushort[] subKey, int depth, bool includeDescendants)
        {
            throw new NotImplementedException();
        }
    }
}
