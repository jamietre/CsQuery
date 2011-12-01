using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    public enum SelectionSetOrder
    {
        OrderAdded=1,
        Ascending=2,
        Descending=3
    }
    /// <summary>
    /// A list of DOM elements, ordered by appearance in the DOM
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectionSet<T>: ISet<T>, IList<T>, ICollection<T>,IEnumerable<T>, IEnumerable where T: IDomObject
    {
        public SelectionSet()
        {
            Order = SelectionSetOrder.OrderAdded;
        }
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
        private HashSet<T> _innerList = null;
        private List<T> _innerListOrdered= null;
        
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

                if (dirty || _sortedList==null)
                {
                    //TODO - right now we copy the list to the target when first accessed in order to ensure its integrity
                    // should the path of an item change (e.g. b/c it's removed from the DOM). Ideally we would not
                    // require this, so the list is only enumerated when needed, but I can't think how to accomplish this easily
                    var comparer = new listOrderComparer(Order);
                    _sortedList = new List<T>(_innerListOrdered.OrderBy(item => item,comparer));
                    dirty = false;
                }
                return _sortedList;
            }
        }
        private class listOrderComparer : IComparer<IDomObject>
        {
            public listOrderComparer(SelectionSetOrder order)
            {
                if (order != SelectionSetOrder.Ascending && order != SelectionSetOrder.Descending)
                {
                    throw new Exception("This comparer can only be used to sort.");
                }
                Order = order;
            }
            protected SelectionSetOrder Order;
            public int Compare(IDomObject x, IDomObject y)
            {
                return Order == SelectionSetOrder.Ascending ?
                    String.CompareOrdinal(x.Path,y.Path) :
                    String.CompareOrdinal(y.Path,x.Path);
            }
        }
        protected IEnumerable<T> _sortedList = null;
        // Make the list dirty, forcing a resort. 
        protected bool dirty = false;
        protected void Touch()
        {
            dirty = true;
        }

        /// <summary>
        /// The order in which elements in the set are returned
        /// </summary>
        public SelectionSetOrder Order {get;set;}

        public IEnumerator<T> GetEnumerator()
        {
            return sortedList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
       

        public bool Add(T item)
        {

            bool result = innerList.Add(item);
            if (result)
            {
                _innerListOrdered.Add(item);
                Touch();
            }
            return result;
            
        }
        void ICollection<T>.Add(T item)
        {
            Add(item);
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
            return _innerList!=null ? 
                innerList.Contains(item) : false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
           innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { 
                return _innerList!=null ? 
                innerList.Count :
                0; 
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
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
        /// <summary>
        /// Use after set operations
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
            // This is just adding things since this list maintains uniqueness
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
                throw new Exception("Index out of range");
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
    }
}
