using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility
{
    public enum SelectionSetOrder
    {
        OrderAdded=1,
        Ascending=2,
        Descending=3
    }
    /// <summary>
    /// A list of DOM elements. The default order is the order added to this construct; the Order property can be changed to
    /// return the contents in a different order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectionSet<T>: ISet<T>, IList<T>, ICollection<T>,IEnumerable<T>, IEnumerable where T: IDomObject
    {
        #region constructor

        public SelectionSet()
        {
            Order = SelectionSetOrder.OrderAdded;
        }

        #endregion

        #region private properties

        // We maintain both a List<T> and a HashSet<T> for selections because set operations are performance-critical
        // for many selectors, but we cannot depend on HashSet<T> to maintain order. If the list is accessed in a sorted
        // order, the sorted version is cached additionally using sortedList.

        private HashSet<T> _innerList = null;
        private List<T> _innerListOrdered = null;
        protected IEnumerable<T> _sortedList = null;

        protected HashSet<T> innerList
        {
            get
            {
                if (_innerList == null)
                {
                    _innerList = new HashSet<T>();
                    _innerListOrdered = new List<T>();
                }
                return _innerList;
            }
        }
        protected IEnumerable<T> sortedList
        {
            get
            {
                if (_innerListOrdered==null) {
                    return Objects.EmptyEnumerable<T>();
                }
                if (Order == SelectionSetOrder.OrderAdded)
                {
                    return _innerListOrdered;
                }

                if (_sortedList==null)
                {
                    //TODO - right now we copy the list to the target when first accessed in order to ensure its integrity
                    // should the path of an item change (e.g. b/c it's removed from the DOM). Ideally we would not
                    // require this, so the list is only enumerated when needed, but I can't think how to accomplish this easily
                    var comparer = new SelectionSetComparer(Order);
                    _sortedList = new List<T>(_innerListOrdered.OrderBy(item => item,comparer));
                }
                return _sortedList;
            }
        }
        

        #endregion

        #region public properties

        /// <summary>
        /// The order in which elements in the set are returned
        /// </summary>
        public SelectionSetOrder Order {get;set;}

        public int Count
        {
            get
            {
                return _innerList != null ?
                    innerList.Count :
                    0;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region public methods

        public bool Add(T item)
        {
            if (innerList.Add(item))
            {
                _innerListOrdered.Add(item);
                Touch();
                return true;
            } else {
                return false;
            }        
        }

        public void Clear()
        {
            if (_innerList!=null)
            {
                innerList.Clear();
                _innerListOrdered.Clear();
                Touch();
            }
        }

        public bool Contains(T item)
        {
            return _innerList != null ?
                innerList.Contains(item) : false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
           innerList.CopyTo(array, arrayIndex);
        }

       
        public bool Remove(T item)
        {
            bool result = false;
            if (_innerList!=null)
            {
                result = innerList.Remove(item);
                if (result)
                {
                    _innerListOrdered.Remove(item);
                    Touch();
                }
            }
            return result;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            innerList.ExceptWith(other);
            SynchronizeOrderedListAfterRemove();
            Touch();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            innerList.IntersectWith(other);
            SynchronizeOrderedListAfterRemove();
            Touch();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return innerList.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return innerList.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return innerList.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return innerList.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return innerList.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return innerList.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            innerList.SymmetricExceptWith(other);
            SynchronizeOrderedListAfterRemove();
            Touch();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            // The hashset maintains uniqueness; we can just try to add everything.

            foreach (T item in other)
            {
                Add(item);
            }
        }

        public int IndexOf(T item)
        {
            return _innerListOrdered.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (innerList.Add(item))
            {
                _innerListOrdered.Insert(index, item);
                Touch();
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= Count || Count==0)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            T item = _innerListOrdered[index];
            innerList.Remove(item);
            _innerListOrdered.RemoveAt(index);
            Touch();
        }

        public T this[int index]
        {
            get
            {
                return _innerListOrdered[index];
            }
            set
            {
                T item = _innerListOrdered[index];
                _innerListOrdered[index] = value;
                innerList.Remove(item);
                innerList.Add(value);
                Touch();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return sortedList.GetEnumerator();
        }

        #endregion

        #region private methods

        /// <summary>
        /// Force the list to be re-sorted.
        /// </summary>
        protected void Touch()
        {
            _sortedList = null;
        }

        /// <summary>
        /// Use after set operations that alter the list
        /// </summary>
        private void SynchronizeOrderedListAfterRemove()
        {
            int index = 0;
            while (index < _innerListOrdered.Count)
            {
                if (!_innerList.Contains(_innerListOrdered[index]))
                {
                    _innerListOrdered.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        #endregion

    }
}
