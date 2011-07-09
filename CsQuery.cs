using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Jtc.ExtensionMethods;
using System.Diagnostics;

namespace Jtc.CsQuery
{
    public class CsQuery: IEnumerable<DomElement>
    {
        // Used to manage extension methods by keeping a reference within the base CsQuery to whatever object it creates.
        internal Dictionary<string, object> ExtensionCache
        {
            get
            {
                if (DomRoot != null)
                {
                    return DomRoot.ExtensionCache;
                } else {
                    if (_ExtensionCache == null)
                    {
                        _ExtensionCache = new Dictionary<string, object>();
                    }
                }
                return _ExtensionCache;
            }
        } protected Dictionary<string, object> _ExtensionCache = null;

        /// <summary>
        /// The CsQuery object that represents the actual DOM for which is is a selector. 
        /// </summary>
        protected CsQuery DomRoot { get; set; }

        protected DomElementFactory ElementFactory
        {
            get
            {
                if (_ElementFactory == null)
                {
                    _ElementFactory = new DomElementFactory();
                }
                return (_ElementFactory);
            }
        } private DomElementFactory _ElementFactory = null;
        /// <summary>
        /// Creates a new instance representing a DOM.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CsQuery Create(string html)
        {
            CsQuery csq = new CsQuery();
            csq.CreateDomImpl(html);

            return csq;
        }

        protected void CreateDomImpl(string html)
        {
            Dom = null;
            Dom.AddRange(ElementFactory.CreateObjects(html));
            AddSelectionRange(Dom.Children);
        }



        /// <summary>
        /// Create a new CsQuery object in a given DOM context
        /// </summary>
        /// <param name="element"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static CsQuery CreateFromElement(DomObject element)
        {
            CsQuery csq = new CsQuery();
            csq.Dom.Add(element);
            csq.AddSelection(element);
            return csq;
        }
        public static CsQuery CreateFromElements(IEnumerable<DomObject> elements)
        {
            CsQuery csq = new CsQuery();
            csq.Dom.AddRange(elements);
            csq.AddSelectionRange(elements);
            return csq;
        }
        /// <summary>
        /// Creates a new, empty jQuery object.
        /// </summary>
        public CsQuery()
        {
            
        }
        /// <summary>
        /// Create a new CsQuery object using an existing instance and a selector
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
        public CsQuery(string selector, CsQuery context)
        {
            // when creating a new CsQuery from another, leave Dom blank - it will be populated automatically with the
            // contents of the selector.
            DomRoot = context;

            if (selector == "*" || selector == null)
            {
                AddSelectionRange(context.Selection);
            }
            else
            {
                Selectors = new CsQuerySelectors(selector);
                AddSelectionRange(Selectors.GetMatches(context.Selection));
            }
        }
        /// <summary>
        /// Create a new CsQuery from a single DOM element
        /// </summary>
        /// <param name="element"></param>
        public CsQuery(DomObject element, CsQuery context)
        {
            DomRoot = context;
            AddSelection(element);
        }
        /// <summary>
        /// Create a new CsQuery object from a set of DOM elements, based on an existing DOM
        /// </summary>
        /// <param name="elements"></param>
        public CsQuery(IEnumerable<DomObject> elements, CsQuery context)
        {
            DomRoot = context;
            AddSelectionRange(elements);
        }
        /// <summary>
        /// Create a new CsQuery object from a set of DOM elements
        /// </summary>
        /// <param name="elements"></param>
        public CsQuery(CsQuery csq)
        {
            DomRoot = csq;
        }




        /// <summary>
        /// Represents the full, parsed DOM for an object created with an HTML parameter
        /// </summary>
        public DomRoot Dom 
        {
            get {
                if (_Dom == null)
                {
                    if (DomRoot != null)
                    {
                        _Dom = DomRoot.Dom;
                    }
                    else
                    {
                        _Dom = new DomRoot();
                        _Dom.Root = _Dom;
                    }
                }
                return _Dom;
            }
            set
            {
                _Dom = value;
                ClearSelections();
                if (_Dom != null)
                {
                    AddSelectionRange(_Dom.Children);
                }
            }
        } protected DomRoot _Dom = null;
        /// <summary>
        /// Only elements that match the selection. 
        /// </summary>
        protected IEnumerable<DomObject> Selection
        {
            get
            {
                if (_Selection == null)
                {
                    _Selection = new List<DomObject>();
                }
                return _Selection;
            }
        }
        protected List<DomObject> _Selection = new List<DomObject>();
        private HashSet<DomObject> _SelectionUnique = new HashSet<DomObject>();
        protected void AddSelection(DomObject element) {
            if (_SelectionUnique.Add(element))
            {
                _Selection.Add(element);
            }
        }
        protected void AddSelectionRange(IEnumerable<DomObject> elements)
        {
            foreach (DomObject elm in elements)
            {
                AddSelection(elm);
            }
        }
        protected void ClearSelections()
        {
            _Selection.Clear();
            _SelectionUnique.Clear();
        }

        protected IEnumerable<DomElement> SelectedElements
        {
            get
            {
                foreach (DomObject obj in Selection)
                {
                    if (obj is DomElement)
                    {
                        yield return (DomElement)obj;
                    }
                }
            }
        }

        public int Length
        {
            get
            {
                return _Selection.Count;
            }
        }
        /// <summary>
        /// Return matched element. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DomElement this[int index]
        {
            get {
                if (_Selection[index] is DomElement)
                {
                    return (DomElement)_Selection[index];
                }
                else
                {
                    throw new Exception("Only Element nodes can be accessed by the default indexer, please use GetElement to access other node types.");
                }
            }
        }
        public DomObject GetElement(int index)
        {
            return _Selection[index];
        }
        /// <summary>
        ///  The selector (parsed) used to create this instance
        /// </summary>
        public CsQuerySelectors Selectors
        {
            get
            {
                return _Selectors;
            }
            protected set
            {
                _Selectors = value;
            }
        } protected CsQuerySelectors _Selectors = null;



        /// <summary>
        /// Set the HTML contents of each element in the set of matched elements.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public CsQuery Html(string html)
        {
            DomRoot newElements = new DomRoot(ElementFactory.CreateObjects(html));
            foreach (DomElement obj in Selection)
            {
                if (!obj.InnerHtmlAllowed)
                {
                    throw new Exception("Attempted to add Html to element with tag '" + obj.Tag + "' which is not allowed.");
                }
                obj.RemoveChildren();
                obj.AddRange(newElements.Children);
            }
            return this;
        }
        /// <summary>
        /// Get the HTML contents of the first element in the set of matched elements.
        /// </summary>
        /// <returns></returns>
        public string Html()
        {
            if (Length > 0)
            {
                return this[0].InnerHtml;
            }
            else
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// Returns the full HTML for all selected documents
        /// </summary>
        /// 
        public string SelectionHtml()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DomObject elm in this)
            {
                sb.Append(sb.Length == 0 ? String.Empty : ", ");
                sb.Append(elm.Html);
            }
            return sb.ToString();
        }
        /// <summary>
        /// Returns markup for the selected elements, *excluding* any inner HTML or children.
        /// </summary>
        /// <returns></returns>
        public string SelectionElements()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DomObject elm in this)
            {
                sb.Append(sb.Length == 0 ? String.Empty : ", ");
                if (elm is DomElement) {
                    sb.Append(((DomElement)elm).ElementHtml);
                } else {
                    sb.Append(elm.Html);
                }
            }
            return sb.ToString();
        }

                /// <summary>
        /// Renders the DOM to a string
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            return Dom.Html;
        }
        /// <summary>
        /// Add elements
        /// </summary>
        /// <param name="?"></param>
        /// <param name="selector"></param>
        protected IEnumerable<DomObject> _FilterElements(IEnumerable<DomElement> elements, string selector)
        {

            if (selector != null)
            {
                CsQuerySelectors selectors = new CsQuerySelectors(selector);
                return selectors.GetMatches(elements);
            }
            else
            {
                return elements;
            }
        }
        #region jQuery Methods
        /// <summary>
        ///  Add elements to the set of matched elements. Returns a NEW jQuery object
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public CsQuery Add(IEnumerable<DomElement> elements)
        {
            CsQuery res = new CsQuery(this);
            res.AddSelectionRange(elements);
            return this;
        }
        /// <summary>
        /// Add elements to the set of matched elements from an HTML fragment. Returns a new jQuery object.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public CsQuery Add(string html)
        {
            return Add(ElementFactory.CreateElements(html));
        }

        /// <summary>
        /// Insert content, specified by the parameter, to the end of each element in the set of matched elements.
        /// TODO: Add overloads with multiple values
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public CsQuery Append(string content)
        {
            foreach (DomElement obj in SelectedElements)
            {
                obj.AddRange(ElementFactory.CreateObjects(content));
            }
            return this;
        }
        public CsQuery Append(CsQuery elements) {
            foreach (var obj in SelectedElements)
            {
                foreach (var e in elements.Selection )
                {
                    obj.Add(e);
                }
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">
        /// delegate(int index, string html) A function that returns an HTML string to insert at the end of each element in the set of matched elements. 
        /// Receives the index position of the element in the set and the old HTML value of the element as arguments.
        /// </param>
        /// <returns></returns>
        public CsQuery Append(Func<int, string, string> func)
        {
            int index = 0;
            foreach (DomElement obj in SelectedElements)
            {
                string clientValue = func(index, obj.InnerHtml);
                IEnumerable<DomObject> objects = ElementFactory.CreateObjects(clientValue);
                obj.AddRange(objects); ;
                index++;
            }
            return this;
        }
        /// <summary>
        /// Get the value of an attribute for the first element in the set of matched elements.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Attr(string name)
        {
            if (Length > 0)
            {
                return this[0].GetAttribute(name, null);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Set one or more attributes for the set of matched elements.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CsQuery Attr(string name, object value)
        {
            bool remove = false;
            string val = String.Empty;
            if (value is bool)
            {
                if (!(bool)value)
                {
                    remove = true;
                }
            }
            else
            {
                val = value.ToString();
            }
            foreach (DomElement e in SelectedElements)
            {
                if (remove)
                {
                    e.RemoveAttribute(name);
                }
                else
                {
                    e.SetAttribute(name, val);
                }
            }
            return this;
        }
        /// <summary>
        /// Insert content, specified by the parameter, before each element in the set of matched elements.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public CsQuery Before(string selector)
        {
            CsQuery csq = new CsQuery(selector, this);
            return Before(csq);

        }
        public CsQuery Before(CsQuery selection)
        {
            foreach (DomObject element in this)
            {
                int index = GetElementIndex(element);
                foreach (DomObject obj in selection)
                {
                    element.Parent.Insert(index, obj);
                    index++;
                }
            }
            return this;
        }
        /// <summary>
        /// Get the children of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns></returns>
        public CsQuery Children()
        {
            return Children(null);
        }
        public CsQuery Children(string selector)
        {
            List<DomElement> list = new List<DomElement>();
            foreach (DomElement obj in SelectedElements)
            {
                list.AddRange(obj.Elements);
            }

            return new CsQuery(_FilterElements(list, selector), this);
        }
        /// <summary>
        /// Create a deep copy of the set of matched elements.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public CsQuery Clone()
        {
            CsQuery csq = new CsQuery();
            foreach (DomElement e in SelectedElements)
            {
                csq.AddSelection(e.Clone());
            }
            return csq;
        }

        /// <summary>
        ///  Set one or more CSS properties for the set of matched elements.
        /// </summary>
        /// <param name="cssJson"></param>
        /// <returns></returns>
        public CsQuery Css(string cssJson)
        {
            return this.Each((DomElement e) =>
            {
                Dictionary<string, string> dict = FromJson(cssJson);
                foreach (var key in dict)
                {
                    e.AddStyle(key.Key, key.Value);
                }
            });
        }
        /// <summary>
        ///  Set one or more CSS properties for the set of matched elements.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CsQuery Css(string name, string value)
        {
            string style = String.Empty;

            foreach (DomElement e in SelectedElements)
            {
                e.AddStyle(name, value);
            }
            return this;
        }

        /// <summary>
        /// Get the value of a style property for the first element in the set of matched elements
        ///  NOTE: This is the equivalent of Css( name ) in the original API. Because we cannot overlaod with signatures
        ///  only differing by return type, the method name must be changed.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public string CssGet(string style)
        {
            if (Length == 0)
            {
                return null;
            }
            else
            {
                return this[0].GetStyle(style);
            }
        }
        /// <summary>
        /// Iterate over each matched element.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public CsQuery Each(Action<int, DomObject> func)
        {
            int index = 0;
            foreach (DomObject obj in Selection)
            {
                func(index, obj);
            }
            return this;
        }
        public CsQuery Each(Action<int, DomElement> func)
        {
            int index = 0;
            foreach (DomElement obj in Selection)
            {
                func(index, obj);
            }
            return this;
        }
        /// <summary>
        /// Iterate over each matched element.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public CsQuery Each(Action<DomElement> func)
        {
            foreach (DomElement obj in SelectedElements)
            {
                func(obj);
            }
            return this;
        }
        public CsQuery Each(Action<DomObject> func)
        {
            foreach (DomObject obj in Selection)
            {
                func(obj);
            }
            return this;
        }
        /// <summary>
        /// Reduce the set of matched elements to the one at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CsQuery Eq(int index)
        {
            if (index < 0)
            {
                index = Length + index;
            }
            return new CsQuery(this[index],this);
        }

        /// <summary>
        /// Get the descendants of each element in the current set of matched elements, filtered by a selector, jQuery object, or element.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery Find(string selector)
        {
            CsQuery csq = new CsQuery(this);
            CsQuerySelectors  selectors = new CsQuerySelectors(selector);
            foreach (DomElement elm in SelectedElements )
            {
                if (elm.Count > 0)
                {
                    csq.AddSelectionRange(selectors.GetMatches(elm.Children));
                }
            }

            return csq;
        }
        /// <summary>
        /// Updates the current object with a new selection
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery Select(string selector)
        {
            CsQuerySelectors selectors = new CsQuerySelectors(selector);
            ClearSelections();
            AddSelectionRange(selectors.Select(Dom));
            return this;
        }
        /// <summary>
        /// Reduce the set of matched elements to the first in the set.
        /// </summary>
        /// <returns></returns>
        public CsQuery First()
        {
            return Eq(0);
        }
        /// <summary>
        /// Hide the matched elements.
        /// </summary>
        /// <returns></returns>
        public CsQuery Hide()
        {
            return this.Each((DomElement e) =>
            {
                e.AddStyle("display", "none");
            });
        }
        /// <summary>
        /// Insert every element in the set of matched elements after the target.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public CsQuery InsertAfter(DomElement element)
        {
            element.Parent.Insert(GetElementIndex(element)+1, element);
            return this;
        }
        public CsQuery InsertAfter(CsQuery obj) {
            bool isFirst = true;
            foreach (DomElement e in obj) {
                if (isFirst)
                {
                    InsertAfter(e);
                    isFirst = false;
                }
                else
                {
                    DomElement clone = e.Clone();
                    InsertAfter(clone);
                }
            }
            return this;
        }
        /// <summary>
        /// Get the immediately following sibling of each element in the set of matched elements. 
        /// If a selector is provided, it retrieves the next sibling only if it matches that selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery Next(string selector)
        {
            List<DomElement> list = new List<DomElement>();
            foreach (DomElement obj in SelectedElements)
            {
                // Extraordinarily inefficient way to get next. TODO: make structure a linked list
                List<DomObject> children = obj.Parent._Children ;
                DomElement next=null;
                for (int i=0;i<children.Count;i++)
                {
                    if (ReferenceEquals(children[i],obj))
                    {
                        while (++i < children.Count)
                        {
                            if (children[i] is DomElement)
                            {
                                next = (DomElement)children[i];
                                i = children.Count;
                            }
                        }
                    }
                }
                if (next != null)
                {
                    list.Add(next);
                }
            }
            return new CsQuery(_FilterElements(list, selector), this);
        }
        public CsQuery Next()
        {
            return Next(null);
        }

        /// <summary>
        /// Get the parent of each element in the current set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery Parent()
        {
            return Parent(null);
        }
        public CsQuery Parent(string selector)
        {
            List<DomElement> list = new List<DomElement>();

            foreach (DomElement obj in SelectedElements)
            {
                if (obj.Parent is DomElement)
                {
                    list.Add((DomElement)obj.Parent);
                }
            }
            return new CsQuery(_FilterElements(list, selector), this);
        }
        /// <summary>
        /// Get the immediately preceding sibling of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns></returns>
        public CsQuery Prev()
        {
            return Prev(null);
        }
        public CsQuery Prev(string selector)
        {
            List<DomElement> list = new List<DomElement>();
            foreach (DomElement obj in SelectedElements)
            {
                // Extraordinarily inefficient way to get next. TODO: make structure a linked list
                List<DomObject> children = obj.Parent._Children;
                DomElement prev = null;
                for (int i = children.Count-1; i >= 0; i--)
                {
                    if (ReferenceEquals(children[i], obj))
                    {
                        while (--i >= 0)
                        {
                            if (children[i] is DomElement)
                            {
                                prev = (DomElement)children[i];
                                i = -1;
                            }
                        }
                    }
                }
                if (prev != null)
                {
                    list.Add(prev);
                }
            }
            return new CsQuery(_FilterElements(list, selector), this);
        }
   
        /// <summary>
        /// Remove the element from the DOM
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public CsQuery Remove(DomObject element)
        {
            if (element.Parent == null)
            {
                throw new Exception("The element is not part of a DOM.");
            }
            element.Parent.Remove(element);
            return this;
        }
        /// <summary>
        /// Remove all elements matching the selector from the DOM
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery Remove()
        {
            for (int i=Length-1;i>=0;i--)
            {
                Remove(this[i]);
            }
            return this;
        }
        public CsQuery Remove(string selector)
        {
            CsQuerySelectors selectors = new CsQuerySelectors(selector);
            foreach (DomElement e in selectors.GetMatches(SelectedElements))
            {
                Remove(e);
            }
            return this;
        }
        /// <summary>
        /// Replace each element in the set of matched elements with the provided new content.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery ReplaceWith(string selector)
        {
            CsQuery newContent = new CsQuery(selector, this);
            return Before(selector).Remove();
        }
        public CsQuery ReplaceWith(CsQuery selection)
        {
            return Before(selection).Remove();
        }
        public CsQuery Show()
        {
            foreach (DomElement e in SelectedElements)
            {
                e.RemoveStyle("display");
            }
            return this;
        }
        /// <summary>
        /// Get the current value of the first element in the set of matched elements.
        /// </summary>
        /// <returns></returns>
        public string Val()
        {
            if (Length > 0)
            {
                DomElement e = this[0];
                switch(e.Tag) {
                    case "textarea":
                        return e.InnerHtml;
                    case "input":
                        switch(e.Type) {
                            default:
                                return e.GetAttribute("value");
                        }
                    default:
                        return e.GetAttribute("value");
                }
            }
            else
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// Set the value of each element in the set of matched elements.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CsQuery Val(string value)
        {
            foreach (DomElement e in SelectedElements)
            {
                switch (e.Tag)
                {
                    case "textarea":
                        // should we delete existing children first? they should not exist
                        e.RemoveChildren();
                        DomLiteral lit = new DomLiteral(value);
                        e.Add(lit);
                        break;
                    case "input":
                        switch (e.Type)
                        {
                            default:
                                e.SetAttribute("value", value);
                                break;
                        }
                        break;
                    default:
                        e.SetAttribute("value", value);
                        break;
                }

            }
            return this;
        }
   
        /// <summary>
        /// Check the current matched set of elements against a selector and return true if at least one of these elements matches the selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public bool Is(string selector)
        {
            CsQuerySelectors selectors = new CsQuerySelectors(selector);
            if (selectors.GetMatches(Selection).IsNullOrEmpty())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
#endregion
        /// <summary>
        /// Parses SIMPLE (non-nested) object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        protected Dictionary<string, string> FromJson(string json)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            StringScanner scanner = new StringScanner(json);
            scanner.Expect("{");
            string data = scanner.Seek("}");

            string[] kvps = data.Split(',');
            foreach (string item in kvps)
            {
                string[] kvp = item.Split(':');
                scanner.Text = kvp[0];
                scanner.AllowQuoting();
                string key = scanner.Seek().Trim();
                scanner.Text = kvp[1];
                scanner.AllowQuoting();
                string value = scanner.Seek().Trim();
                dict.Add(key, value);
            }
            return dict;
        }
        
        protected int GetElementIndex(DomObject element) {
            int count = 0;
            foreach (DomObject e in element.Parent.Children)
            {
                if (ReferenceEquals(e, element))
                {
                    break;
                }
                count++;
            }
            return count;
        }
        #region IEnumerable<DomObject> Members

        public IEnumerator<DomElement> GetEnumerator()
        {
            return SelectedElements.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Selection.GetEnumerator();
        }

        #endregion
    }


    
}
