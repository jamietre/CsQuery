using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Implementation;
using CsQuery.HtmlParser;

namespace CsQuery.Engine
{
    /// <summary>
    /// DOM index that only stores the index target. This will perform much better than the ranged
    /// index for dom construction &amp; manipulation, but worse for complex queries.
    /// 
    /// TODO: Work in progress/
    /// </summary>

    public class DomIndex: IDomIndex
    {
        /// <summary>
        /// Default constructor for the index
        /// </summary>

        public DomIndex()
        {
            Index = new Dictionary<ushort[], HashSet<IDomObject>>(PathKeyComparer.Comparer);
        }

        private IDictionary<ushort[], HashSet<IDomObject>> Index;


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
            HashSet<IDomObject> existing;
            if (!Index.TryGetValue(key, out existing))
            {
                existing = new HashSet<IDomObject>();
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
            HashSet<IDomObject> existing;
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
            throw new NotImplementedException();
            //return SelectorXref.GetRange(subKey);
        }

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>

        public void Clear()
        {
            //SelectorXref.Clear();
            //_PendingIndexChanges = null;
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
                throw new NotImplementedException();
            }
        }

     

    }
}
