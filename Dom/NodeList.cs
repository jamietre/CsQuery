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

        protected void MoveElement(IDomObject element)
        {
 
            // If this element does not have children, ChildNodes will return null and an
            // exception will result - so don't bother checking to improve perofrmance.
            
            // Must always remove from the DOM. It doesn't matter if it's the same DOM or not. An element can't be part of two DOMs - that would require a clone.
            // If it is the same DOM, it would remain a child of the old parent as well as the new one if we didn't remove first.
            if (element.ParentNode != null)
            {
                element.ParentNode.RemoveChild(element);
            }
            DomObject el = (DomObject)element;
            if (!(element is IDomRoot)) {
                el.ParentNode = Owner;
            }

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
            MoveElement(item);
            InnerList.Insert(index, item);
            Reindex(index);
        }
        /// <summary>
        /// Remove an item from this list and update index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            
            if (InnerList[index].NodeType == NodeType.ELEMENT_NODE)
            {
                DomElement item = InnerList[index] as DomElement;

                if (!item.IsDisconnected)
                {
                    item.Document.RemoveFromIndex(item);

                }
                item.ParentNode = null;
            }
            else
            {
                ((DomObject)InnerList[index]).ParentNode=null;
            }
            InnerList.RemoveAt(index);
            Reindex(index);
        }
                        
        
        //Reindex all documents > index (used after inserting)
        protected void Reindex(int index)
        {
            if (InnerList.Count - 1 >= index)
            {
                bool disconnected = InnerList[index].IsDisconnected;
                for (int i = index; i < InnerList.Count; i++)
                {
                    if (InnerList[i].NodeType == NodeType.ELEMENT_NODE)
                    {
                        var el = (DomElement)InnerList[i];
                        if (!disconnected)
                        {
                            el.Document.RemoveFromIndex(el);
                        }
                    }
                }
                for (int i = index; i < InnerList.Count; i++)
                {
                    if (InnerList[i].NodeType == NodeType.ELEMENT_NODE)
                    {
                        var el = (DomElement)InnerList[i];

                        // This would get assigned anyway but this is much faster since we already know the index
                        el._Index = i;
                        if (!disconnected)
                        {
                            el.Document.AddToIndex(el);
                        }
                    }
                    else
                    {
                        ((DomObject)InnerList[i])._Index = i;
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
               /// <summary>
        /// Add a child to this element 
        /// </summary>
        /// <param name="element"></param>
        public void Add(IDomObject item)
        {
            MoveElement(item);
          
            InnerList.Add(item);
            ((DomObject)item)._Index = InnerList.Count-1;
            if (item.NodeType == NodeType.ELEMENT_NODE && !item.IsDisconnected)
            {
                IDomElement el = (IDomElement)item;
                el.Document.AddToIndex(el);
            }
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
            if (item.ParentNode != this.Owner) {
                return false;
            } else {
                RemoveAt(item.Index);
                return true;
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
