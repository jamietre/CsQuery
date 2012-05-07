using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    public class NodeList: INodeList
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

        protected void RemoveParent(IDomObject element)
        {
            if (element.ParentNode != null)
            {
                if (!element.IsDisconnected && element is IDomIndexedNode)
                {
                    element.Document.RemoveFromIndex((IDomIndexedNode)element);
                }
                ((DomObject)element).ParentNode = null;
            }
            

        }
        protected void AddParent(IDomObject element, int index)
        {
            DomObject item = (DomObject)element;
            item.ParentNode = Owner;
            item.Index = index;
            if (element is IDomIndexedNode && !element.IsDisconnected)
            {
                element.Document.AddToIndex((IDomIndexedNode)element);
            }
        }
        #region IList<T> Members

        public int IndexOf(IDomObject item)
        {
            return _InnerList == null ? -1 : InnerList.IndexOf(item);
        }
        /// <summary>
        /// Add a child to this element 
        /// </summary>
        /// <param name="element"></param>
        public void Add(IDomObject item)
        {
 
            if (item.ParentNode != null)
            {
                item.Remove();
            }
            // Ensure ID uniqueness - remove ID if same-named object already exists
            if (!String.IsNullOrEmpty(item.Id) 
                && !Owner.IsDisconnected 
                && Owner.Document.GetElementById(item.Id)!=null) 
            {
                item.Id = null;
            }
            InnerList.Add(item);
            AddParent(item, InnerList.Count - 1);
        }
        /// <summary>
        /// Adds a child element at a specific index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="element"></param>
        public void Insert(int index, IDomObject item)
        {
            //RemoveParent(item);
            if (item.ParentNode != null)
            {
                item.Remove();
            }
            if (index == InnerList.Count)
            {
                InnerList.Add(item);
            }
            else
            {
                InnerList.Insert(index, item);
            }
            // This must come BEFORE AddParent - otherwise the index entry will be present already at this position 
            Reindex(index + 1); 
            AddParent(item, index);
        }
        /// <summary>
        /// Remove an item from this list and update index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            IDomObject item = InnerList[index];
            InnerList.RemoveAt(index);
            RemoveParent(item);
            Reindex(index);
        }

        /// <summary>
        /// Remove an element from this element's children
        /// </summary>
        /// <param name="element"></param>
        public bool Remove(IDomObject item)
        {
            if (item.ParentNode != this.Owner)
            {
                return false;
            }
            else
            {
                RemoveAt(item.Index);
                return true;
            }
        }
        //Reindex all documents > index (used after inserting, when relative index among siblings changes)
        protected void Reindex(int index)
        {
            if (index < InnerList.Count)
            {
                bool isDisconnected = Owner.IsDisconnected;
                //bool oldIsDisconnected = InnerList[index].IsDisconnected;

                for (int i = index; i < InnerList.Count; i++)
                {
                    if (!isDisconnected && InnerList[i].NodeType == NodeType.ELEMENT_NODE)
                    {
                        var el = (DomElement)InnerList[i];

                        // This would get assigned anyway but this is much faster since we already know the index
                        Owner.Document.RemoveFromIndex(el);
                        el.Index = i;
                        Owner.Document.AddToIndex(el);
                    }
                    else
                    {
                        ((DomObject)InnerList[i]).Index = i;
                    }
                }
            }
        }
        public IDomObject this[int index]
        {
            get
            {
                return InnerList[index];
            }
            set
            {
                RemoveAt(index);
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
