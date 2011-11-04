using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    public class NodeList: IList<IDomObject>, ICollection<IDomObject>, IEnumerable<IDomObject> 
    {
        public NodeList(IDomContainer owner)
        {
            Owner = owner;
        }
        protected  IDomContainer Owner;
        protected List<IDomObject> InnerList
        {
            get
            {
                if (_InnerList == null)
                {
                    _InnerList = new List<IDomObject>();
                }
                return _InnerList;
            }
        }
        protected List<IDomObject> _InnerList = null;

        protected void PrepareElement(IDomObject element)
        {
            if (!Owner.InnerHtmlAllowed && !Owner.InnerTextAllowed)
            {
                throw new Exception("Cannot add children to this element type. InnerHTMLAllowed: " + Owner.InnerHtmlAllowed + " InnerTextAllowed: " + Owner.InnerTextAllowed );
            }
            // Must always remove from the DOM. It doesn't matter if it's the same DOM or not. An element can't be part of two DOMs - that would require a clone.
            // If it is the same DOM, it would remain a child of the old parent as well as the new one if we didn't remove first.
            if (element.ParentNode != null)
            {
                element.ParentNode.RemoveChild(element);
            }

            // Set owner recursively
            element.Owner = Owner.Owner;
            element.ParentNode = Owner;
        }

        #region IList<T> Members

        public int IndexOf(IDomObject item)
        {
            return _InnerList == null ? -1 : InnerList.IndexOf(item);
        }
        /// <summary>
        /// Adds a child element at a specific index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="element"></param>
        public void Insert(int index, IDomObject item)
        {
            PrepareElement(item);
            InnerList.Insert(index, item);
            item.AddToIndex();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public IDomObject this[int index]
        {
            get
            {
                return InnerList[index];
            }
            set
            {
                IDomObject el = InnerList[index];
                Remove(el);
                if (index < InnerList.Count)
                {
                    Insert(index, value);
                }
                else
                {
                    Add(value);
                }

            }
        }

        #endregion

        #region ICollection<IDomObject> Members
               /// <summary>
        /// Add a child to this element 
        /// </summary>
        /// <param name="element"></param>
        public void Add(IDomObject item)
        {
            PrepareElement(item);
          
            InnerList.Add(item);
            item.AddToIndex();
        }
        public void AddRange(IEnumerable<IDomObject> elements)
        {
            // because elements will be removed from their parent while adding, we need to copy the
            // enumerable first since it will change otherwise
            List<IDomObject> copy = new List<IDomObject>(elements);
            foreach (IDomObject e in copy)
            {
                Add(e);
            }
        }
        /// <summary>
        /// Remove all children of this node
        /// </summary>
        public void Clear()
        {
            if (_InnerList != null)
            {
                for (int i = InnerList.Count - 1; i >= 0; i--)
                {
                    Remove(InnerList[i]);
                }
            }
        }

        public bool Contains(IDomObject item)
        {
            return _InnerList==null ? false : InnerList.Contains(item);
        }

        public void CopyTo(IDomObject[] array, int arrayIndex)
        {
            InnerList.CopyTo(array,arrayIndex);
        }

        public int Count
        {
            get { return _InnerList==null ? 0 : InnerList.Count; }
        }

        public int Length
        {
            get { return Count; }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Remove an element from this element's children
        /// </summary>
        /// <param name="element"></param>
        public bool Remove(IDomObject item)
        {
            if (_InnerList != null && InnerList.Remove(item))
            {
                item.RemoveFromIndex();

                item.ParentNode = null;
                item.Owner = null;
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<IDomObject> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<IDomObject> IEnumerable<IDomObject>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


    }
}
