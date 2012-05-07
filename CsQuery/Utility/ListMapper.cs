using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility
{
    
    /// <summary>
    /// Maps a list from one type to another. Elements of the wrong type are returned as null.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    class MapperEnumerator<T> : IEnumerator<T>
    {
        IList SourceList;
        int index = -1;
        public MapperEnumerator(IList sourceList)
        {
            SourceList = sourceList;
        }
        public T Current
        {
            get {
                return (T)SourceList[index];
            }
        }

        public void Dispose()
        {
            SourceList = null;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            return (++index < SourceList.Count);
        }

        public void Reset()
        {
            index  =-1;
        }
    }
    public class ListMapper<T>: IList<T>, IList
    {
        private IList _SourceList;
        IList SourceList
        {
            get
            {
                return _SourceList;
            }
            set
            {
                _SourceList = value;
                enumerator = new MapperEnumerator<T>(_SourceList);
            }
        }
        IEnumerator<T> enumerator;
        public ListMapper(IList sourceList)
        {
            SourceList = sourceList;
            
        }
        public ListMapper(IList sourceList, bool reduce)
        {

            if (reduce)
            {
                SourceList = (IList)ReduceToMatches(sourceList);
            }
            else
            {
                SourceList = sourceList;
            }
        }
        /// <summary>
        /// Filters the list to only include matching types
        /// </summary>
        /// <returns></returns>
        public void ReduceToMatches()
        {
            SourceList = (IList)ReduceToMatches(SourceList);
        }
        protected IList<T> ReduceToMatches(IList list)
        {
            List<T> reduced = new List<T>();
            foreach (var el in list)
            {
                if (el is T)
                {
                    reduced.Add((T)el);
                }
            }
            return reduced;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return enumerator;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }




        public int IndexOf(T item)
        {
            return SourceList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            SourceList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                var item =SourceList[index];
                if (item is T)
                {
                    return (T)item;
                }
                else
                {
                    return default(T);
                }
            }
            set
            {
                SourceList[index] = value;
            }
        }

        public void Add(T item)
        {

            SourceList.Add(item);
        }

        public void Clear()
        {
            SourceList.Clear();
        }

        public bool Contains(T item)
        {
            return SourceList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            SourceList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return SourceList.Count; }
        }

        public bool IsReadOnly
        {
            get { return SourceList.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            int count = SourceList.Count;
            SourceList.Remove(item);
            return SourceList.Count != count;
        }


        int IList.Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return IsReadOnly; }
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (T)value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        int ICollection.Count
        {
            get { return Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return false; }
        }
    }
}
