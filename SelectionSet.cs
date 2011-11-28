using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    /// <summary>
    /// A list of DOM elements, ordered by appearance in the DOM
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectionSet<T>: ISet<T>, ICollection<T>,IEnumerable<T>, IEnumerable where T: IDomObject
    {
        public SelectionSet()
        {
            IsSorted = true;
        }
        protected HashSet<T> innerList
        {
            get
            {
                if (_innerList == null)
                {
                    _innerList = new HashSet<T>();
                }
                return _innerList;
            }
        }
        private HashSet<T> _innerList = null;
        
        protected IEnumerable<T> sortedList
        {
            get
            {
                if (!IsSorted)
                {
                    return innerList;
                }

                if (dirty || _sortedList==null)
                {
                    _sortedList = innerList.OrderBy(item => item.Path);
                }
                return _sortedList;
            }
        }
        protected IEnumerable<T> _sortedList = null;
        // Make the list dirty, forcing a resort. 
        protected bool dirty = false;
        protected void Touch()
        {
            dirty = true ;
        }
        /// <summary>
        /// When true, elements are returned in DOM order. Otherwise, they are returned in the order added.
        /// </summary>
        public bool IsSorted
        {
            get;
            set;
        }
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
                    Touch();
                }
            }
            return result;
        }


        public void ExceptWith(IEnumerable<T> other)
        {
            innerList.ExceptWith(other);
            Touch();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            innerList.IntersectWith(other);
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
            Touch();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            innerList.UnionWith(other);
            Touch();
        }
    }
}
