using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Implementation
{
    /// <summary>
    /// An INodeList wrapper for an IList object
    /// </summary>
    ///
    /// <typeparam name="T">
    /// Generic type parameter.
    /// </typeparam>

    public class NodeList<T>: INodeList<T> where T: IDomObject
    {
        #region constructor

        public NodeList(IList<T> list)
        {
            InnerList = list;
            IsReadOnly = true;
        }

        #endregion

        #region private properties

        protected IList<T> InnerList;

        #endregion


        public int Length
        {
            get { return InnerList.Count; }
        }

        public T Item(int index)
        {
            return this[index];
        }

        public int IndexOf(T item)
        {
            return InnerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            InnerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
        }
        
        [IndexerName("Indexer")]
        public T this[int index]
        {
            get
            {
                return InnerList[index];

            }
            set
            {
                InnerList[index] = value;
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
            get;
            protected set;
        }

        public bool Remove(T item)
        {
            return InnerList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
