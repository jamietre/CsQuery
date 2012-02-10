using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility
{
    /// <summary>
    /// Note: Currently unused by CsQuery as of 2/1/2012. Delete in a couple months.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoList<T> : IList<T>
    {
        /// <summary>
        /// Permits accessing the next indexed member by index for set/insert operations by using Add automatically.
        /// Also returns Null for unindexed values instead of an error.
        /// </summary>
        protected List<T> InnerList = new List<T>();

        public int IndexOf(T item)
        {
            return InnerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (index == Count)
            {
                Add(item);
            }
            else
            {
                InnerList.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    return default(T);
                }
                return InnerList[index];
            }
            set
            {
                if (index == Count)
                {
                    Add(value);
                }
                else
                {
                    InnerList[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            InnerList.Add(item);
        }

        public void Clear()
        {
            InnerList.Clear();
        }

        public bool Contains(T item)
        {
            return InnerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return InnerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }
    }
}
