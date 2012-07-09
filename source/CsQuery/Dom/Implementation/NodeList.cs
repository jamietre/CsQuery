﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    public class NodeList: INodeList
    {
        #region constructor

        public NodeList(IDomContainer owner)
        {
            Owner = owner;
        }

        #endregion

        #region private properties

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

        #endregion

        #region public properties

        public IDomContainer Owner { get; set; }

        #endregion


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
            //if (item.Document != Owner.Document)
            //{
            //    ((DomObject)item).Document = null;
            //}

            // Ensure ID uniqueness - remove ID if same-named object already exists
            if (!String.IsNullOrEmpty(item.Id)
                && !Owner.IsFragment
                && Owner.Document.GetElementById(item.Id) != null)
            {
                item.Id = null;
            }

            InnerList.Add(item);
            AddParent(item, InnerList.Count - 1);
        }
        /// <summary>
        /// Add a child without validating that a node is a member of this DOM already or that the ID is unique
        /// </summary>
        /// <param name="item"></param>
        public void AddAlways(IDomObject item)
        {
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

        #region private methods


        protected void RemoveParent(IDomObject element)
        {
            if (element.ParentNode != null)
            {
                DomObject item = element as DomObject;
                if (!element.IsDisconnected && element.IsIndexed)
                {
                    item.Document.DocumentIndex.RemoveFromIndex((IDomIndexedNode)element);
                }
                ((DomObject)element).ParentNode = null;
            }


        }
        
        protected void AddParent(IDomObject element, int index)
        {
            DomObject item = element as DomObject;

            item.ParentNode = Owner;
            item.Index = index;
            if (element.IsIndexed)
            {
                item.Document.DocumentIndex.AddToIndex((IDomIndexedNode)element);
            }
        }

        //Reindex all documents > index (used after inserting, when relative index among siblings changes)
        protected void Reindex(int index)
        {
            if (index < InnerList.Count)
            {
                bool isDisconnected = Owner.IsDisconnected;

                for (int i = index; i < InnerList.Count; i++)
                {
                    if (!isDisconnected && InnerList[i].NodeType == NodeType.ELEMENT_NODE)
                    {
                        var el = (DomElement)InnerList[i];

                        // This would get assigned anyway but this is much faster since we already know the index
                        el.Document.DocumentIndex.RemoveFromIndex(el);
                        el.Index = i;

                        Owner.Document.DocumentIndex.AddToIndex(el);
                    }
                    else
                    {
                        ((DomObject)InnerList[i]).Index = i;
                    }
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

        public IEnumerator<IDomObject> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        #endregion


    }
}
