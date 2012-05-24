using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;

namespace CsQuery
{
    public partial class CQ
    {
        #region private properties

        protected CQ _CsQueryParent;
        protected SelectionSet<IDomObject> _Selection;
        
        /// <summary>
        /// The object from which this CsQuery was created
        /// </summary>
        protected CQ CsQueryParent
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
                else
                {
                    Document = null;
                }
                ClearSelections();
            }
        }

        /// <summary>
        /// The current selection set including all node types. 
        /// </summary>
        protected SelectionSet<IDomObject> SelectionSet
        {
            get
            {
                if (_Selection == null)
                {
                    _Selection = new SelectionSet<IDomObject>();
                    _Selection.Order = SelectionSetOrder.OrderAdded;
                }
                return _Selection;
            }
        }

        #endregion

        #region private methods
        /// <summary>
        /// Clear the entire object
        /// </summary>
        protected void Clear()
        {
            CsQueryParent = null;
            Document = null;
            ClearSelections();
        }
        /// <summary>
        /// Clears the current selection set
        /// </summary>
        protected void ClearSelections()
        {
            SelectionSet.Clear();
        }
        /// <summary>
        /// Map a CSV or enumerable object to a hashset
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// Helper function for option groups to set multiple options when passed a CSV of values.
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
                        Objects.Join((IEnumerable)value) : value.ToString()
                    )
                );
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
            //    throw new InvalidOperationException("Cannot add unbound elements or elements bound to another DOM directly to a selection set.");
            //}
            return SelectionSet.Add(element);
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
        /// Helper function for Attr & Prop
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void SetProp(string name, object value)
        {
            bool state = Objects.IsTruthy(value);
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
        protected CQ ForEach(IEnumerable<IDomObject> source, Func<IDomObject, IDomObject> del)
        {
            CQ output = New();
            foreach (var item in SelectionSet)
            {
                output.SelectionSet.Add(del(item));
            }
            return output;
        }
        protected CQ ForEachMany(IEnumerable<IDomObject> source, Func<IDomObject, IEnumerable<IDomObject>> del)
        {
            CQ output = New();
            foreach (var item in SelectionSet)
            {
                output.SelectionSet.AddRange(del(item));
            }
            return output;
        }
        /// <summary>
        /// Runs a set of selectors and returns the combined result as a single enumerable
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> mergeSelections(IEnumerable<string> selectors)
        {
            SelectionSet<IDomObject> allContent = new SelectionSet<IDomObject>();
            Each(selectors, item => allContent.AddRange(Select(item)));
            return allContent;
        }
        /// <summary>
        /// Runs a set of HTML creation selectors and returns result as a single enumerable
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> mergeContent(IEnumerable<string> content)
        {
            List<IDomObject> allContent = new List<IDomObject>();
            foreach (var item in content)
            {
                allContent.AddRange(CQ.Create(item));
            }
            return allContent;
        }
        protected CQ filterIfSelector(string selector, IEnumerable<IDomObject> list)
        {
            return filterIfSelector(selector, list,SelectionSetOrder.OrderAdded);
        }
        protected CQ filterIfSelector(string selector,IEnumerable<IDomObject> list, SelectionSetOrder order)
        {
            CQ output;
            if (String.IsNullOrEmpty(selector))
            {
                output= new CQ(list, this);
            }
            else
            {
                output= new CQ(filterElements(list, selector), this);
            }
            output.Order = order;
            return output;
        }
        protected IEnumerable<IDomElement> onlyElements(IEnumerable<IDomObject> objects)
        {
            foreach (var item in objects)
            {
                IDomElement el = item as IDomElement;
                if (el!=null) {
                    yield return el;
                }
            }
        }
        protected IEnumerable<IDomObject> filterElements(IEnumerable<IDomObject> elements, string selector)
        {
            return filterElementsIgnoreNull(elements, selector ?? "");
        }
        //<summary>
        // Filter an element list using another selector. A null selector results in no filtering.
        //</summary>
        //<param name="?"></param>
        //<param name="selector"></param>
        protected IEnumerable<IDomObject> filterElementsIgnoreNull(IEnumerable<IDomObject> elements, string selector)
        {
            if (selector=="")
            {
                return Objects.EmptyEnumerable<IDomObject>();
            }
            else if (selector == null)
            {
                return elements;
            }
            else
            {
                SelectorChain selectors = new SelectorChain(selector);
                if (selectors.Count > 0)
                {
                    selectors.Do(item=>item.TraversalType = TraversalType.Filter);
                }
                // this is kind of unfortunate but is required to keep the order correct. Probably a more efficient
                // way to do it but works fine for now
                
                return elements.Intersect(selectors.Select(Document, elements));
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
        /// Insert every element in the selection at or after the index of each target (adding offset to the index).
        /// If there is more than one target, the a clone is made of the selection for the 2nd and later targets.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected CQ InsertAtOffset(IEnumerable<IDomObject> target, int offset)
        {
            SelectionSet<IDomObject> sel = target as SelectionSet<IDomObject>;
            bool isCsQuery = sel != null;

            bool isFirst = true;

            // Copy the target list: it could change otherwise
            List<IDomObject> targets = new List<IDomObject>(target);

            if (isCsQuery && sel.Count == 0)
            {
                // If appending items to an empty selection, just add them to the selection set
                sel.AddRange(SelectionSet);
            }
            else
            {
                foreach (var el in targets)
                {
                    if (el.IsDisconnected)
                    {
                        // Disconnected items are added to the selection set (if that's the target)
                        if (!isCsQuery)
                        {
                            throw new InvalidOperationException("You can't add elements to a disconnected element list, it must be in a selection set");
                        }
                        int index = sel.IndexOf(el);
                        foreach (var item in SelectionSet)
                        {
                            sel.Insert(index + offset, item);
                        }
                    }
                    else
                    {
                        if (isFirst)
                        {
                            InsertAtOffset(el, offset);
                            isFirst = false;
                        }
                        else
                        {
                            Clone().InsertAtOffset(el, offset);
                        }
                    }
                }
            }
            return this;
        }
        /// <summary>
        /// Deals with tbody as the target of appends
        /// </summary>
        /// <param name="apparentTarget"></param>
        /// <returns></returns>
        protected IDomElement getTrueTarget(IDomElement target)
        {
            //Special handling for tables: make sure we add to the TBODY
            IDomElement element = target;
            if (target.NodeName == "table")
            {
                bool addBody = false;
                if (target.HasChildren)
                {
                    IDomElement body = target.ChildElements.FirstOrDefault(item => item.NodeName == "tbody");
                    if (body != null) {
                        element = body;
                    }
                    else if (target.FirstElementChild == null)
                    {
                        // Add a body if there are no elements in this table yet
                        addBody = true;
                    }
                    // default = leave it alone, they've already added elements. don't worry whether it's valid or not, 
                    // assume they know what they're doing.
                }
                else
                {
                    addBody = true;
                }
                if (addBody)
                {
                    element = Document.CreateElement("tbody");
                    target.AppendChild(element);
                }
            }
            return element;
        }

       
        #endregion
        
        
    }
}
