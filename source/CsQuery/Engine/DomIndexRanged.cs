using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Implementation;
using CsQuery.HtmlParser;

namespace CsQuery.Engine
{
    /// <summary>
    /// An index that can return a range of values
    /// </summary>

    public class DomIndexRanged: IDomIndexRanged
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public DomIndexRanged()
        {
            QueueChanges = true;
        }

        private RangeSortedDictionary<ushort, IDomObject> _SelectorXref;


        private Queue<IndexOperation> _PendingIndexChanges;


        /// <summary>
        /// The nodes that have changed since the last Reindex
        /// </summary>

        private Queue<IndexOperation> PendingIndexChanges
        {
            get
            {
                if (_PendingIndexChanges == null)
                {
                    _PendingIndexChanges = new Queue<IndexOperation>();
                }
                return _PendingIndexChanges;
            }
        }

        /// <summary>
        /// Returns true when there are pending index changes
        /// </summary>

        private bool IndexNeedsUpdate
        {
            get
            {
                return _PendingIndexChanges != null & PendingIndexChanges.Count > 0;
            }
        }

        /// <summary>
        /// The index.
        /// </summary>

        internal RangeSortedDictionary<ushort, IDomObject> SelectorXref
        {
            get
            {
                if (_SelectorXref == null)
                {
                    _SelectorXref = new RangeSortedDictionary<ushort, IDomObject>(PathKeyComparer.Comparer,
                        PathKeyComparer.Comparer,
                        HtmlData.indexSeparator);
                }
                return _SelectorXref;
            }
        }

        /// <summary>
        /// When true, changes are queued until the next read operation.
        /// </summary>

        public bool QueueChanges { get; set; }

        /// <summary>
        /// Add an element to the index using the default keys for this element.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to add
        /// </param>

        public void AddToIndex(IDomIndexedNode element)
        {
            foreach (ushort[] key in element.IndexKeysRanged())
            {
                QueueAddToIndex(key, element);
            }

            if (element.HasChildren)
            {
                foreach (DomElement child in ((IDomContainer)element).ChildElements)
                {
                    AddToIndex(child);
                }
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
            QueueAddToIndex(key, element);
        }
        /// <summary>
        /// Remove an element from the index using its key.
        /// </summary>
        ///
        /// <param name="key">
        /// The key to remove
        /// </param>

        public void RemoveFromIndex(ushort[] key)
        {
            QueueRemoveFromIndex(key);
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

            foreach (ushort[] key in element.IndexKeysRanged())
            {
                QueueRemoveFromIndex(key);
            }
        }

        /// <summary>
        /// Query the document's index for a subkey up to a specific depth, optionally including
        /// descendants that match the selector.
        /// </summary>
        ///
        /// <param name="subKey">
        /// The key or subkey to match. If this is a partial key, all keys matching this part will be
        /// returned.
        /// </param>
        /// <param name="depth">
        /// The zero-based depth to which searches should be limited.
        /// </param>
        /// <param name="includeDescendants">
        /// When true, descendants of matching keys will be returned
        /// </param>
        ///
        /// <returns>
        /// A sequence of all matching keys
        /// </returns>

        public IEnumerable<IDomObject> QueryIndex(ushort[] subKey, int depth, bool includeDescendants)
        {
            ProcessQueue();
            return SelectorXref.GetRange(subKey, depth, includeDescendants);
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
            ProcessQueue();
            return SelectorXref.GetRange(subKey);
        }

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>

        public void Clear()
        {
            SelectorXref.Clear();
            _PendingIndexChanges = null;
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
                ProcessQueue();
                return SelectorXref.Count;
            }
        }

        /// <summary>
        /// Add an element to the index using a specified index key.
        /// </summary>
        ///
        /// <param name="key">
        /// The key
        /// </param>
        /// <param name="element">
        /// The element target
        /// </param>

        private void QueueAddToIndex(ushort[] key, IDomIndexedNode element)
        {
            if (QueueChanges)
            {
              
                PendingIndexChanges.Enqueue(new IndexOperation
                {
                    Key = key,
                    Value = element.IndexReference,
                    IndexOperationType = IndexOperationType.Add
                });
            }
            else
            {
                SelectorXref.Add(key, element.IndexReference);
            }
        }
        private void QueueRemoveFromIndex(ushort[] key)
        {
            if (QueueChanges)
            {

                PendingIndexChanges.Enqueue(new IndexOperation
                {
                    Key = key,
                    IndexOperationType = IndexOperationType.Remove
                });
            }
            else
            {
                SelectorXref.Remove(key);
            }
        }

        private void ProcessQueue()
        {
            if (_PendingIndexChanges == null)
            {
                return;
            }

            while (PendingIndexChanges.Count > 0)
            {
                var item = PendingIndexChanges.Dequeue();

                switch (item.IndexOperationType)
                {
                    case IndexOperationType.Add:
                        SelectorXref.Add(item.Key, item.Value);
                        break;
                    case IndexOperationType.Remove:
                        SelectorXref.Remove(item.Key);
                        break;
                }
            }
        }
    }
}
