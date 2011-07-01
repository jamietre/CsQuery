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

        protected CsQuery DomRoot = null;

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
            CsQuery root = new CsQuery();
            root.CreateDomImpl(html);
            // add jus the real Elements, any literals should not be part of the object
            
            root._Elements.AddRange(root.Dom.Elements);
            return root;
        }
        /// <summary>
        /// Create a new CSQuery object for a single element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static CsQuery CreateFromElement(DomElement element)
        {
            DomObject obj = element.Parent;
            while (obj.Parent != null)
            {
                obj = obj.Parent;
            }
            //TODO use a static dictionary to cache this

            CsQuery root = new CsQuery();
            root.Dom = (DomRoot)obj;
            root._Elements.Add(element);
            return root;
        }
        /// <summary>
        /// Creates a new, empty jQuery object.
        /// </summary>
        public CsQuery()
        {
            
        }
        public static CsQuery CreateFromElement(DomElement element, CsQuery context)
        {
            CsQuery csq = new CsQuery();
            csq.DomRoot = context;
            csq._Elements.Add(element);
            return csq;
        }
        public static CsQuery CreateFromElements(IEnumerable<DomElement> elements, CsQuery context)
        {
            CsQuery csq = new CsQuery();
            csq.DomRoot = context;
            csq._Elements.AddRange(elements);
            return csq;
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
                _Elements.AddRange(context.Elements);
            }
            else
            {
                Selectors = new CsQuerySelectors(selector);
                _Elements.AddRange(Selectors.GetMatches(context.Elements));
            }
        }
        /// <summary>
        /// Create a new CsQuery object from a set of DOM elements, based on an existing DOM
        /// </summary>
        /// <param name="elements"></param>
        public CsQuery(IEnumerable<DomElement> elements, CsQuery context)
        {
            DomRoot = context;
            _Elements.AddRange(elements);
        }
        /// <summary>
        /// Create a new CsQuery object from a set of DOM elements
        /// </summary>
        /// <param name="elements"></param>
        public CsQuery(IEnumerable<DomElement> elements)
        {
            _Elements.AddRange(elements);
        }

        protected void CreateDomImpl(string html)
        {
            Dom.Clear();
            Dom.Add(ElementFactory.CreateObjects(html));
        }

        //protected string SourceHtml
        //{
        //    get
        //    {
        //        return _SourceHtml;
        //    }
        //    set
        //    {
        //        _SourceHtml = value;
        //    }
        //} internal string _SourceHtml = String.Empty;

        /// <summary>
        /// Represents the full, parsed DOM for an object created with an HTML parameter
        /// </summary>
        public DomRoot Dom 
        {
            get {
                if (_Dom == null)
                {
                    _Dom = new DomRoot(this);
                }
                return _Dom;
            }
            protected set
            {
                _Dom = value;
            }
        } protected DomRoot _Dom = null;
        /// <summary>
        /// The elements that match the selection. 
        /// </summary>
        protected IEnumerable<DomElement> Elements
        {
            get
            {
                return _Elements;
            }
        } 
        protected List<DomElement> _Elements 
        {
            get
            {
                if (__Elements == null)
                {
                    __Elements = new List<DomElement>();
                }
                return __Elements;
            }
        } protected List<DomElement> __Elements = null;

        public int Length
        {
            get
            {
                return _Elements.Count;
            }
        }
        public DomElement this[int index]
        {
            get {
                return _Elements[index];
            }
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
            foreach (DomElement obj in Elements)
            {
                if (!obj.InnerHtmlAllowed)
                {
                    throw new Exception("Attempted to add Html to element with tag '" + obj.Tag + "' which is not allowed.");
                }
                obj.RemoveChildren();
                obj.Add(newElements.Children);
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
                return _Elements[0].InnerHtml;
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
                sb.Append(elm.Html);
            }
            return sb.ToString();
        }
        /// <summary>
        /// Returns the full HTML for the document root
        /// </summary>
        /// <returns></returns>
        public string DomHtml()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DomObject elm in Dom)
            {
                sb.Append(elm.Html);
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
        protected IEnumerable<DomElement> _FilterElements(IEnumerable<DomElement> elements, string selector)
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
        public CsQuery Find(string selector)
        {
            return new CsQuery(selector,this);
        }
        /// <summary>
        /// Iterate over each matched element.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public CsQuery Each(Action<int, DomElement> func)
        {
            int index=0;
            foreach (DomElement obj in Elements)
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
            foreach (DomElement obj in Elements)
            {
                func(obj);
            }
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
            return CsQuery.CreateFromElement(_Elements[index]);
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
            foreach (DomElement obj in Elements)
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
            foreach (DomElement obj in Elements)
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
            foreach (DomElement obj in Elements)
            {
                list.AddRange(obj.Elements);
            }
         
            return new CsQuery(_FilterElements(list,selector),this);
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

            foreach (DomElement obj in Elements)
            {
                if (obj.Parent is DomElement)
                {
                    list.Add((DomElement)obj.Parent);
                }
            }
            return new CsQuery(_FilterElements(list,selector), this);
        }
        /// <summary>
        ///  Add elements to the set of matched elements. Returns a NEW jQuery object
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public CsQuery Add(IEnumerable<DomElement> elements)
        {
            CsQuery res = new CsQuery(Elements);
            res.Dom.Add(elements);
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
            foreach (DomElement obj in Elements)
            {
                obj.Add(ElementFactory.CreateObjects(content));
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
        public CsQuery Append(Func<int,string,string> func)
        {
            int index = 0;
            foreach (DomElement obj in Elements)
            {
                string clientValue = func(index, obj.InnerHtml);
                IEnumerable<DomObject> objects = ElementFactory.CreateObjects(clientValue);
                obj.Add(objects); ;
                index++;
            }
            return this;
        }
        /// <summary>
        /// Remove the element from the DOM
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public CsQuery Remove(DomElement element)
        {
            if (element.Parent == null)
            {
                throw new Exception("The element is not part of a DOM.");
            }
            element.Parent.Remove(element);
            return this;
        }
        /// <summary>
        /// Remove all the elements from the DOM
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public CsQuery Remove(IEnumerable<DomElement> elements)
        {
            foreach (DomElement e in elements) 
            {
                Remove(e);
            }
            return this;
        }
        /// <summary>
        /// Remove all elements matching the selector from the DOM
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery Remove(string selector)
        {
            CsQuerySelectors selectors = new CsQuerySelectors(selector);
            foreach (DomElement e in selectors.GetMatches(Elements)) {
                Remove(e);
            }
            return this;
        }
        public CsQuery InsertAfter(DomElement element)
        {
            int count = 0;
            foreach (DomElement e in element.Parent.Elements)
            {
                if (e == element)
                {
                    break;
                } 
                count++;
            }
            element.Parent.Insert(count, element);
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
                DomElement e = _Elements[0];
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
            foreach (DomElement e in Elements)
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
        public CsQuery Attr(string name,object value)
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
            foreach (DomElement e in Elements)
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
        /// Check the current matched set of elements against a selector and return true if at least one of these elements matches the selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public bool Is(string selector)
        {
            CsQuerySelectors selectors = new CsQuerySelectors(selector);
            if (selectors.GetMatches(Elements).IsNullOrEmpty())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
#endregion

        #region IEnumerable<DomObject> Members

        public IEnumerator<DomElement> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        #endregion
    }


    
}
