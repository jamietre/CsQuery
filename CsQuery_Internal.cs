using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery
{
    public partial class CsQuery
    {
        // Used to manage extension methods by keeping a reference within the base CsQuery to whatever object it creates.
        internal static Dictionary<string, object> ExtensionCache
        {
            get
            {
                if (_ExtensionCache == null)
                {
                    _ExtensionCache = new Dictionary<string, object>();
                }
                
                return _ExtensionCache;
            }
        } 
        protected static Dictionary<string, object> _ExtensionCache = null;

        //protected DomElementFactory ElementFactory
        //{
        //    get
        //    {
        //        if (_ElementFactory == null || !ReferenceEquals(_ElementFactory.Document,Document))
        //        {
        //            _ElementFactory = new DomElementFactory(Document);
        //        }
        //        return (_ElementFactory);
        //    }
        //} private DomElementFactory _ElementFactory = null;

        /// <summary>
        /// The object from which this CsQuery was created
        /// </summary>
        protected CsQuery CsQueryParent
        {
            get
            {
               
                {
                    return _CsQueryParent;
                }
            }
            set
            {
                _CsQueryParent = value;
                if (value != null)
                {
                    Document = value.Document;
                }
            }
        }
        protected void Clear()
        {
            CsQueryParent = null;
            Document = null;
            ClearSelections();
        }
        protected CsQuery _CsQueryParent;

        /// <summary>
        /// The current selection set including all node types. 
        /// </summary>
        protected SelectionSet<IDomObject> Selection
        {
            get
            {
                if (_Selection == null)
                {
                    _Selection = new SelectionSet<IDomObject>();
                }
                return _Selection;
            }
        }
        protected SelectionSet<IDomObject> _Selection = null;

        /// <summary>
        /// Returns just IDomElements from the selection list.
        /// </summary>
        public IEnumerable<IDomElement> Elements
        {
            get
            {
                foreach (IDomObject obj in Selection)
                {
                    if (obj.NodeType==NodeType.ELEMENT_NODE)
                    {
                        yield return (IDomElement)obj;
                    }
                }
            }
        }

        #region Internal Support Functions

        /// <summary>
        /// Return a CsQuery object wrapping the enumerable passed, or the object itself if 
        /// already a CsQuery obect
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        protected CsQuery EnsureInWrapper(IEnumerable<IDomObject> elements)
        {
            return elements is CsQuery ? (CsQuery)elements : new CsQuery(elements);
        }
        /// <summary>
        /// Return the relative position of an element among its Element siblings (non-element nodes excluded)
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected int GetElementIndex(IDomObject element)
        {
            int count = 0;
            IDomContainer parent = element.ParentNode;
            if (parent == null)
            {
                count = -1;
            }
            else
            {
                foreach (IDomElement el in parent.ChildElements)
                {
                    if (ReferenceEquals(el, element))
                    {
                        break;
                    }
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// Add an item to the list of selected elements. It should be part of this DOM.
        /// </summary>
        /// <param name="element"></param>
        protected bool AddSelection(IDomObject element)
        {
            //if (!ReferenceEquals(element.Dom, Dom))
            //{
            //    throw new Exception("Cannot add unbound elements or elements bound to another DOM directly to a selection set.");
            //}
            return Selection.Add(element);
        }
        /// <summary>
        /// Adds each element to the current selection set. Returns true if any elements were added.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        protected bool AddSelectionRange(IEnumerable<IDomObject> elements)
        {
            bool result = false;
            foreach (IDomObject elm in elements)
            {
                result = true;
                AddSelection(elm);
            }
            return result;
        }
        /// <summary>
        /// Clears the current selection set
        /// </summary>
        protected void ClearSelections()
        {
            Selection.Clear();
        }
        /// <summary>
        /// Helper function for Attr & Prop
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void SetProp(string name, object value)
        {
            bool state = value.IsTruthy();
            foreach (IDomElement e in Elements)
            {
                if (state)
                {
                    e.SetAttribute(name);
                }
                else
                {
                    e.RemoveAttribute(name);
                }
            }
        }

        protected CsQuery FilterIfSelector(IEnumerable<IDomObject> list, string selector)
        {

            if (String.IsNullOrEmpty(selector))
            {
                return new CsQuery(list, this);
            }
            else
            {
                return new CsQuery(_FilterElements(list, selector), this);
            }
        }
        
        /// <summary>
        /// Return selection from within an element list
        /// </summary>
        /// <param name="?"></param>
        /// <param name="selector"></param>
        protected IEnumerable<IDomObject> _FilterElements(IEnumerable<IDomObject> elements, string selector)
        {
            if (selector.IsNullOrEmpty())
            {
                return Objects.EmptyEnumerable<IDomObject>();
            }
            else
            {
                CsQuerySelectors selectors = new CsQuerySelectors(selector);
                if (selectors.Count > 0)
                {
                    selectors[0].TraversalType = TraversalType.Filter;
                }
                return selectors.Select(Document, elements);

            }
        }
        /// <summary>
        /// Return all children of all selected elements
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IDomObject> SelectionChildren()
        {
            foreach (IDomObject obj in Elements)
            {
                foreach (IDomObject child in obj.ChildElements)
                {
                    yield return child;
                }
            }
        }
        /// <summary>
        /// Return all children of all selected elements
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IDomObject> SelectionSiblings()
        {
            HashSet<IDomObject> siblings = new HashSet<IDomObject>();
            foreach (IDomObject obj in Elements)
            {
                foreach (IDomObject child in obj.ParentNode.ChildNodes)
                {
                    yield return child;
                }
            }
        }
        /// <summary>
        /// Ouptuts the deepest-nested object, it's root element from the list of elements passed,
        /// and returns the depth, given a structure
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        protected int getInnermostContainer(IEnumerable<IDomElement> elements, 
            out IDomElement element,
            out IDomElement rootElement)
        {
            int depth = 0;
            element = null;
            rootElement = null;
            foreach (IDomElement el in elements)
            {
                if (el.HasChildren)
                {
                    IDomElement innerEl;
                    IDomElement root;
                    int innerDepth = getInnermostContainer(el.ChildElements, 
                        out innerEl,
                        out root);
                    if (innerDepth > depth)
                    {
                        depth = innerDepth + 1;
                        element = innerEl;
                        rootElement = el;
                    }
                }
                if (depth == 0)
                {
                    depth = 1;
                    element = el;
                    rootElement = el;
                } 
            }
            return depth;
        }
        /// <summary>
        /// if true, then next elements are returned, otherwise, previous
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> AdjacentElements(bool getNext)
        {
            foreach (IDomElement obj in Elements)
            {
                // Extraordinarily inefficient way to get next. TODO: make structure a linked list

                IDomElement last = null;
                var children = obj.ParentNode.ChildElements.GetEnumerator();
                //children.Reset();
                bool found = false;
                while (children.MoveNext())
                {

                    if (found && children.Current.NodeType == NodeType.ELEMENT_NODE)
                    {
                        yield return (IDomElement)children.Current;
                        break;
                    }
                    if (ReferenceEquals(children.Current, obj))
                    {
                        if (!getNext)
                        {
                            yield return last;
                            break;
                        }
                        else
                        {
                            found = true;
                        }
                    }
                    last = (IDomElement)children.Current;
                }

            }
            yield break;
        }
        protected CsQuery InsertAtOffset(IEnumerable<IDomObject> target, int offset)
        {
            bool isFirst = true;
            foreach (IDomObject e in target)
            {
                if (e is IDomElement)
                {
                    if (isFirst)
                    {
                        InsertAtOffset(e, offset);
                        isFirst = false;
                    }
                    else
                    {
                        Clone().InsertAtOffset(e, offset);
                    }
                }
            }
            return this;
        }
        #endregion
        protected HashSet<string> MapMultipleValues(object value)
        {
            var values = new HashSet<string>();
            if (value is string)
            {
                values.AddRange(value.ToString().Split(','));

            }
            if (value is IEnumerable)
            {
                foreach (object obj in (IEnumerable)value)
                {
                    values.Add(obj.ToString());
                }
            }

            if (values.Count == 0)
            {
                if (value != null)
                {
                    values.Add(value.ToString());
                }
            }
            return values;

        }
        /// <summary>
        /// Helper function for option groups. I am sure these can be simplified
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="value"></param>
        /// 
        protected void SetOptionSelected(IEnumerable<IDomElement> elements, object value, bool multiple)
        {
            HashSet<string> values = MapMultipleValues(value);
            SetOptionSelected(elements, values, multiple);
        }
        protected void SetOptionSelected(IEnumerable<IDomElement> elements, HashSet<string> values, bool multiple)
        {
            bool setOne = false;
            string attribute;

            foreach (IDomElement e in elements)
            {
                attribute = String.Empty;
                switch (e.NodeName)
                {
                    case "option":
                        attribute = "selected";
                        break;
                    case "input":
                        switch (e["type"])
                        {
                            case "checkbox":
                            case "radio":
                                attribute = "checked";
                                break;
                        }
                        break;
                    case "optgroup":
                        SetOptionSelected(e.ChildElements, values, multiple);
                        break;
                }
                if (attribute != String.Empty && !setOne && values.Contains(e["value"]))
                {
                    e.SetAttribute(attribute);
                    if (!multiple)
                    {
                        setOne = true;
                    }
                }
                else
                {
                    e.RemoveAttribute(attribute);
                }

            }
        }
        protected string GetValueString(object value)
        {
            return value == null ? null :
                (value is string ? (string)value :
                    (value is IEnumerable ?
                        ((IEnumerable)value).Join() : value.ToString()
                    )
                );
        }
    }
}
