using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Jtc.ExtensionMethods;
using Jtc.Scripting;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Jtc.CsQuery
{
    /// <summary>
    /// Relationship between Dom and Selections
    /// 
    /// Dom represents a "DOM" and is shared among related CsQuery objects. This is the actual heirarchy. When a subquery is run, it 
    /// uses the same DOM as its parent. 
    /// Selections is a set of elements matching a selector. It is always a subset of DOM. 
    /// 
    /// DomRoot is a reference to the original CsQuery object that a DOM was created. 
    /// 
    /// Only CsQuery.Create creates a new DOM. Any other methods create unbound elements.
    /// Cloning elements also creates unbound elements. They are not part of any DOM until added. So removing something from a clone will have no effect on 
    /// a DOM.
    /// </summary>
    
    public class CsQuery : IEnumerable<IDomObject>
    {
        // Used to manage extension methods by keeping a reference within the base CsQuery to whatever object it creates.
        internal Dictionary<string, object> ExtensionCache
        {
            get
            {
                if (DomOwner != null)
                {
                    return DomOwner.ExtensionCache;
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
        /// The CsQuery object that contains the actual DOM for which is is a selector. 
        /// </summary>
        protected CsQuery DomOwner { get; set; }

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
        /// Creates a new DOM. This will DESTROY any existing DOM. This is not the same as Select.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public void CreateNew(string html)
        {
            _Dom = new DomRoot();
            _Dom.Owner = this;
            ClearSelections();
            Dom.ChildNodes.AddRange(ElementFactory.CreateObjects(html));
            AddSelectionRange(Dom.ChildNodes);
        }
        /// <summary>
        /// Creates a new DOM. This will DESTROY any existing DOM. This is not the same as Select.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public void CreateNew(IEnumerable<IDomObject> elements)
        {
            _Dom = new DomRoot();
            _Dom.Owner = this;
            ClearSelections();
            Dom.ChildNodes.AddRange(elements);
            AddSelectionRange(Dom.ChildNodes);
        }
        
        //public static CsQuery CreateFromElements(IEnumerable<IDomObject> elements)
        //{
        //    CsQuery csq = new CsQuery();
        //    csq.Dom.AddRange(elements);
        //    csq.AddSelectionRange(elements);
        //    return csq;
        //}
        /// <summary>
        /// Creates a new, empty jQuery object.
        /// </summary>
        public CsQuery()
        {
            
        }
        public CsQuery(string selector)
        {
            CreateNew(selector);
        }
        public CsQuery(string selector, string css)
        {
            CreateNew(selector);
            Css(css);

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
            DomOwner = context;

            //Find(selector);

            if (selector == null)
            {
                AddSelectionRange(context.Children());
            }
            else
            {
                Selectors = new CsQuerySelectors(selector);
                AddSelectionRange(Selectors.Select(Dom,context.Children()));
            }
        }
        /// <summary>
        /// Create a new CsQuery from a single elemement, using its own context
        /// </summary>
        /// <param name="element"></param>
        public CsQuery(IDomObject element)
        {
            DomOwner = element.Owner;
            AddSelection(element);
        }
        /// <summary>
        /// Create a new CsQuery object from a set of DOM elements, using the DOM of the first element.
        /// could contain more than one context)
        /// </summary>
        /// <param name="elements"></param>
        public CsQuery(IEnumerable<IDomObject> elements)
        {
            IDomObject el;
            if (!elements.TryGetFirst(out el))
            {
                return;
            }
            DomOwner = elements.First().Owner;
            AddSelectionRange(elements);
        }

        /// <summary>
        /// Create a new CsQuery object from a set of DOM elements, using the DOM of the first element.
        /// could contain more than one context)
        /// </summary>
        /// <param name="elements"></param>
        public CsQuery(IEnumerable<IDomObject> elements, CsQuery context)
        {
            DomOwner = context;
            AddSelectionRange(elements);
        }

        /// <summary>
        /// Create a new CsQuery from a single DOM element
        /// </summary>
        /// <param name="element"></param>
        public CsQuery(IDomObject element, CsQuery context)
        {
            DomOwner = context;
            AddSelection(element);
        }

        /// <summary>
        /// Creeate a new DOM from HTML text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CsQuery Create(string html)
        {
            return new CsQuery(html);
        }
        public static CsQuery Create(string html, string css)
        {
            return new CsQuery(html, css);
        }
        /// <summary>
        /// Creates a new DOM from a file
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        public static CsQuery LoadFile(string htmlFile)
        {
            return new CsQuery(Support.GetFile(htmlFile));
        }
        /// <summary>
        /// Creeate a new DOM from elements
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CsQuery Create(IEnumerable<IDomObject> elements)
        {
            CsQuery csq = new CsQuery();
            csq.CreateNew(elements);
            return csq;
        }
        /// <summary>
        /// Mimics .Create
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static implicit operator CsQuery(string html)
        {
            return CsQuery.Create(html);
        }
        /// <summary>
        /// Create a new CsQuery object using the DOM context from an element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static implicit operator CsQuery(DomElement element)
        {
            CsQuery csq = new CsQuery(element);
            return csq;
        }

        /// <summary>
        /// Represents the full, parsed DOM for an object created with an HTML parameter
        /// </summary>
        public DomRoot Dom 
        {
            get {
                if (_Dom == null)
                {
                    if (DomOwner != null)
                    {
                        _Dom = DomOwner.Dom;
                    }
                    else
                    {
                        _Dom = new DomRoot();
                        _Dom.Owner =this;
                    }
                }
                return _Dom;
            }
            //set
            //{
            //    _Dom = value;
            //    ClearSelections();
            //    if (_Dom != null)
            //    {
            //        AddSelectionRange(_Dom.Children);
            //    }
            //}
        } protected DomRoot _Dom = null;
        /// <summary>
        /// Only elements that match the selection. 
        /// </summary>
        protected IEnumerable<IDomObject> Selection
        {
            get
            {
                if (_Selection == null)
                {
                    _Selection = new List<IDomObject>();
                }
                return _Selection;
            }
        }
        protected List<IDomObject> _Selection = new List<IDomObject>();
        private HashSet<IDomObject> _SelectionUnique = new HashSet<IDomObject>();
        
        /// <summary>
        /// Add an item to the list of selected elements. It should be part of this DOM.
        /// </summary>
        /// <param name="element"></param>
        protected void AddSelection(IDomObject element) {
            //if (!ReferenceEquals(element.Dom, Dom))
            //{
            //    throw new Exception("Cannot add unbound elements or elements bound to another DOM directly to a selection set.");
            //}
            if (_SelectionUnique.Add(element))
            {
                _Selection.Add(element);
            }
        }
        protected void AddSelectionRange(IEnumerable<IDomObject> elements)
        {
            foreach (IDomObject elm in elements)
            {
                AddSelection(elm);
            }
        }
        protected void ClearSelections()
        {
            _Selection.Clear();
            _SelectionUnique.Clear();
        }
        /// <summary>
        /// Returns just IDomElements from the selection list.
        /// </summary>
        public IEnumerable<IDomElement> Elements
        {
            get
            {
                foreach (IDomObject obj in Selection)
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
        public IDomElement this[int index]
        {
            get {
                return (IDomElement)Get(index);
            }
        }

        
        public CsQuery this[string selector]
        {
            get
            {
                return Select(selector);
            }
        }

        public CsQuery this[IDomObject element]
        {
            get
            {
                return Select(element);
            }
        }
        public CsQuery this[IEnumerable<IDomObject> element]
        {
            get
            {
                return Select(element);
            }
        }
        public IEnumerable<IDomObject> Get()
        {
            return Selection;
        }
        public IDomObject Get(int index)
        {
            int effectiveIndex = index < 0 ? _Selection.Count+index-1 : index;

            if (effectiveIndex >= 0 && effectiveIndex < _Selection.Count)
            {
                return (IDomObject)_Selection[effectiveIndex];
            }
            else
            {
                return null;
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
            foreach (DomElement obj in Selection)
            {
                if (!obj.InnerHtmlAllowed)
                {
                    throw new Exception("Attempted to add Html to element with tag '" + obj.NodeName + "' which is not allowed.");
                }
                obj.ChildNodes.Clear();
                obj.ChildNodes.AddRange(newElements.ChildNodes);
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
        /// Returns the HTML for all selected documents, separated by commas. No inner html or children are included.
        /// </summary>
        /// 
        public string SelectionHtml()
        {
            return SelectionHtml(false);
        }
        /// <summary>
        /// Renders just the selection set completely.
        /// </summary>
        /// <returns></returns>
        public string RenderSelection()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IDomObject elm in this)
            {
                sb.Append(elm.Render());
            }
            return sb.ToString();
        }
        public string SelectionHtml(bool includeInner)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IDomObject elm in this)
            {
                
                sb.Append(sb.Length == 0 ? String.Empty : ", ");
                sb.Append(includeInner ? elm.Render() : elm.ToString());
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return SelectionHtml();
        }
         /// <summary>
        /// Renders the DOM to a string
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            return Dom.Render();
        }
        public string Render(DomRenderingOptions renderingOptions)
        {
            Dom.DomRenderingOptions = renderingOptions;
            return Render();
        }
        /// <summary>
        /// Add elements
        /// </summary>
        /// <param name="?"></param>
        /// <param name="selector"></param>
        protected IEnumerable<IDomObject> _FilterElements(IEnumerable<IDomObject> elements, string selector)
        {

            if (selector != null)
            {
                CsQuerySelectors selectors = new CsQuerySelectors(selector);
                if (selectors.Count > 0)
                {
                    selectors[0].TraversalType = TraversalType.Filter;
                }
                return selectors.Select(Dom,elements);
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
        public CsQuery Add(IEnumerable<IDomElement> elements)
        {
            CsQuery res = new CsQuery(this);
            res.AddSelectionRange(elements);
            return res;
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
        /// Adds the specified class(es) to each of the set of matched elements.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public CsQuery AddClass(string className)
        {
            foreach (var item in Elements)
            {
                item.AddClass(className);
            }
            return this;
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
                obj.ChildNodes.AddRange(ElementFactory.CreateObjects(content));
            }
            return this;
        }
        public CsQuery Append(CsQuery elements) {
            foreach (var obj in Elements)
            {
                foreach (var e in elements.Selection )
                {
                    obj.AppendChild(e);
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
            foreach (DomElement obj in Elements)
            {
                string clientValue = func(index, obj.InnerHtml);
                IEnumerable<IDomObject> objects = ElementFactory.CreateObjects(clientValue);
                obj.ChildNodes.AddRange(objects); ;
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
            foreach (IDomObject element in this)
            {
                int index = GetElementIndex(element);
                foreach (IDomObject obj in selection)
                {
                    element.ParentNode.ChildNodes.Insert(index,obj);
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
            List<IDomElement> list = new List<IDomElement>();
            foreach (IDomElement obj in Elements)
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
            
            foreach (IDomObject elm in Elements)
            {
                IDomObject clone = elm.Clone();
                csq.Dom.AppendChild(clone);
                csq.AddSelection(clone);
                
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
            return this.Each((IDomElement e) =>
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

            foreach (IDomElement e in Elements)
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
                return ((IDomElement)this[0]).GetStyle(style);
            }
        }
        /// <summary>
        /// Returns value at named data store for the first element in the jQuery collection, as set by data(name, value).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public object getData<T>(string key) {
            string json = getData(key);
            return json.fromJSON<T>();
        }
        public string getData(string key) {
            return this.First()[0].GetAttribute("data-"+key);
        }
        public CsQuery Data(string key,string jsonData)
        {
            this.Each((IDomElement e) =>
            {
                e.SetAttribute("data-" + key, jsonData);
            });
            return this;
        }
        public CsQuery Data(string key, object data)
        {
            this.Each((IDomElement e) =>
            {
                e.SetAttribute("data-" + key, data.toJSON());
            });
            return this;
        }
        /// <summary>
        /// Iterate over each matched element.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public CsQuery Each(Action<int, IDomElement> func)
        {
            int index = 0;
            foreach (IDomElement obj in Elements)
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

        public CsQuery Each(Action<IDomElement> func)
        {
            foreach (IDomElement obj in Elements)
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
                index = Length + index-1;
            }
            if (index >= 0 && index < Length)
            {
                return new CsQuery(this[index], this);
            }
            else
            {
                return EmptySelection();
            }
        }
        protected CsQuery EmptySelection()
        {
            CsQuery empty = new CsQuery();
            empty.DomOwner = this;
            return empty;
        }
        /// <summary>
        /// Get the descendants of each element in the current set of matched elements, filtered by a selector, jQuery object, or element.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery Find(string selector)
        {
            CsQuery csq = new CsQuery();
            csq.DomOwner = this;
            CsQuerySelectors selectors = new CsQuerySelectors(selector);
            csq.AddSelectionRange(selectors.Select(Dom,Children()));
            return csq;


        }

        public CsQuery Filter(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Select from the whole DOM and return a new CSQuery object 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery Select(string selector)
       {
            CsQuery csq = new CsQuery();
            csq.DomOwner = this;
            CsQuerySelectors selectors = new CsQuerySelectors(selector);
            csq.AddSelectionRange(selectors.Select(Dom));
            return csq;
        }

        public CsQuery Select(IDomObject element)
        {
            CsQuery csq = new CsQuery(element);
            return csq;
        }
        public CsQuery Select(IEnumerable<IDomObject> elements)
        {
            CsQuery csq = new CsQuery(elements);
            return csq;
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
        /// Reduce the set of matched elements to the last in the set.
        /// </summary>
        /// <returns></returns>
        public CsQuery Last()
        {
            return Eq(_Selection.Count - 1);
        }
        /// <summary>
        /// Hide the matched elements.
        /// </summary>
        /// <returns></returns>
        public CsQuery Hide()
        {
            return this.Each((IDomElement e) =>
            {
                e.AddStyle("display", "none");
            });
        }
        /// <summary>
        /// Insert every element in the set of matched elements after the target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public CsQuery InsertAfter(IDomObject target)
        {
            return InsertAtOffset(target,1);
        }
        public CsQuery InsertAfter(CsQuery target) {
            return InsertAtOffset(target, 1);
        }
        /// <summary>
        /// Support for InsertAfter and InsertBefore. An offset of 0 will insert before the current element. 1 after.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected CsQuery InsertAtOffset(IDomObject target, int offset)
        {
            int index = GetElementIndex(target);
            foreach (var item in this)
            {
                target.ParentNode.ChildNodes.Insert(index+offset,item);
                index++;
            }
            return this;
        }
        public CsQuery InsertAtOffset(CsQuery target, int offset)
        {
            bool isFirst = true;
            foreach (IDomElement e in target)
            {
                if (isFirst)
                {
                    InsertAtOffset(e,offset);
                    isFirst = false;
                }
                else
                {
                    e.Clone();
                    IDomObject clone = e.Clone();
                    InsertAtOffset(clone,offset);
                }
            }
            return this;
        }
        /// <summary>
        /// A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted before the element(s) specified by this parameter.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public CsQuery InsertBefore(IDomObject target)
        {
            return InsertAtOffset(target, 0);
        }
        public CsQuery InsertBefore(CsQuery target)
        {
            return InsertAtOffset(target, 0);
        }

        /// <summary>
        /// Get the immediately following sibling of each element in the set of matched elements. 
        /// If a selector is provided, it retrieves the next sibling only if it matches that selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CsQuery Next(string selector)
        {
            List<IDomElement> list = new List<IDomElement>();
            foreach (IDomElement obj in Elements)
            {
                // Extraordinarily inefficient way to get next. TODO: make structure a linked list
                
                IDomElement next=null;
                var children = obj.ParentNode.ChildNodes.GetEnumerator();
                children.Reset();
                bool found = false;
                while (children.MoveNext())
                {
                    if (found && children.Current.NodeType == NodeType.ELEMENT_NODE)
                    {
                        next = (IDomElement)children.Current;
                        break;
                    }
                    if (ReferenceEquals(children.Current,obj))
                    {
                        found = true;
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
            List<IDomElement> list = new List<IDomElement>();

            foreach (IDomElement obj in Elements)
            {
                if (obj.ParentNode is IDomElement)
                {
                    list.Add((IDomElement)obj.ParentNode);
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
            List<IDomElement> list = new List<IDomElement>();
            foreach (IDomElement obj in Elements)
            {
                // Extraordinarily inefficient way to get next. TODO: make structure a linked list
                
                var children = obj.ParentNode.ChildNodes.GetEnumerator();
                children.Reset();
                IDomElement prev = null;
                while(children.MoveNext()) {
                    
                    if (ReferenceEquals(children.Current, obj))
                    {
                        break;
                    }
                    if (children.Current.NodeType == NodeType.ELEMENT_NODE)
                    {
                        prev = (IDomElement)children.Current;
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
        public CsQuery Remove(IDomObject element)
        {
            if (element.ParentNode == null)
            {
                throw new Exception("The element is not part of a DOM.");
            }
            element.ParentNode.RemoveChild(element);
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
            foreach (IDomElement e in selectors.Select(Dom))
            {
                Remove(e);
            }
            return this;
        }
        
        /// <summary>
        /// Remove a single class, multiple classes, or all classes from each element in the set of matched elements.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public CsQuery RemoveClass(string className)
        {
            foreach (var elm in this.Elements)
            {
                elm.RemoveClass(className);
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
            foreach (IDomElement e in Elements)
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
                IDomElement e = this.Elements.First();
                switch(e.NodeName) {
                    case "textarea":
                        return e.InnerHtml;
                    case "input":
                        switch(e.GetAttribute("type",String.Empty)) {
                            default:
                                return e.GetAttribute("value");
                        }
                    case "select":
                        string result = String.Empty;

                        foreach (IDomElement child in e.Elements)
                        {
                            if (e.NodeName == "option" && e.HasAttribute("selected"))
                            {
                                result += (result == String.Empty ? String.Empty : ",") + e["value"];
                            }
                        }
                        return result;
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
        /// Set the value of each element in the set of matched elements. If a comma-separated value is passed to a multuple select list, then it
        /// will be treated as an array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CsQuery Val(string value)
        {
            foreach (IDomElement e in Elements)
            {
                switch (e.NodeName)
                {
                    case "textarea":
                        // should we delete existing children first? they should not exist
                        e.InnerText = value;
                        break;
                    case "input":
                        switch (e.GetAttribute("type",String.Empty))
                        {
                            default:
                                e.SetAttribute("value", value);
                                break;
                        }
                        break;
                    case "select":
                        bool multiple = e.HasAttribute("multiple");
                        HashSet<string> values=null;
                        if (multiple)
                        {
                            values = new HashSet<string>(value.Split(','));
                        }
                        foreach (IDomElement child in e.Elements)
                        {
                            if (e.NodeName == "option" 
                                && (multiple ? values.Contains(e["value"]) : e["value"] == value))
                            {
                                e.SetAttribute("selected");
                            }
                            else
                            {
                                e.RemoveAttribute("selected");
                            }
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
        /// Set the value of each mutiple select element in the set of matched elements. Any elements not of type &lt;SELECT multiple&gt;&lt;/SELECT&gt; will be ignored.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CsQuery Val(IEnumerable<object> values) {
            string valueString=String.Empty;
            foreach (object val in values) {
                valueString+=(String.IsNullOrEmpty(val.ToString())?String.Empty:",") + val.ToString();
            }
            foreach (IDomElement e in Elements)
            {
                if (e.NodeName == "select" && e.HasAttribute("multiple"))
                {
                    Val(valueString);
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
            if (selectors.Select(Dom,Selection).IsNullOrEmpty())
            {
                return false;
            }
            else
            {
                return true;
                
            }
        }
        public static ExpandoObject Extend(ExpandoObject target, object source1, object source2 = null, object source3 = null, object source4 = null, object source5 = null, object source6 = null, object source7 = null)
        {
            return Extend(false, target, source1, source2, source3, source4, source5, source6, source7);
        }
        public static ExpandoObject Extend(bool deep,ExpandoObject target, object source1, object source2 = null, object source3 = null, object source4 = null, object source5 = null, object source6 = null, object source7 = null)
        {
            // Add all non-null parameters to a processing queue
            Queue<object> sources = new Queue<object>();
            foreach (object obj in new object[] { source1, source2, source3, source4, source5, source6, source7 })
            {
                if (obj != null)
                {
                    sources.Enqueue(obj);
                }
            }
            // Create a new empty object if there's no existing target -- same as using {} as the jQuery parameter
            if (target == null)
            {
                target = new ExpandoObject();
            }

            var targetDic = (IDictionary<string, object>)target;
            int index = 0;
            // use a while because Count may change if an enumerable extends the list
            
            //sources = sources.Dequeue();
            object source;
            while (sources.Count>0) 
            {
                source = sources.Dequeue();
                if (source == null)
                {
                    continue;
                }
                
                if (source is IDictionary<string, object>)
                {
                    // Expando object -- copy/clone it
                    foreach (var kvp in ( IDictionary<string, object>)source) {
                        object curValue;
                        if (deep && kvp.Value.IsExtendableType() && targetDic.TryGetValue(kvp.Key, out curValue))
                        {
                            // we have to start from a null object b/c it may not be an expando object to start with
                            // If the current value is NOT an extendable type, overwrite it instead (e.g. ignore it by passing null)
                            targetDic[kvp.Key]=Extend(true, null, curValue.IsExtendableType() ? curValue : null, kvp.Value);
                        }
                        else
                        {
                            targetDic[kvp.Key] = deep ? kvp.Value.Clone(true) : kvp.Value;
                        }
                    }
                } else if (source is IEnumerable) {
                    // For enumerables, treat each value as another object. Append to the operation list 
                    foreach (object obj in ((IEnumerable)source))
                    {
                        sources.Enqueue(obj);
                    }
                } else {
                    // treat it as a regular object - try to copy fields/properties
                    IEnumerable<MemberInfo> members = source.GetType().GetMembers();
                    foreach (var member in members)
                    {
                        string name = member.Name;
                        object value = null;
                        if (member is PropertyInfo)
                        {
                            value = ((PropertyInfo)member).GetGetMethod().Invoke(source, null);

                        }
                        else if (member is FieldInfo)
                        {
                            value = ((FieldInfo)member).GetValue(source);
                        }
                        else
                        {
                            //It's a method or something we don't know how to handle. Skip it.
                            continue;
                        }
                        object curValue;
                        if (deep && value.IsExtendableType() &&  targetDic.TryGetValue(name, out curValue))
                        {
                            targetDic[name]=Extend(true, null, curValue.IsExtendableType() ? curValue : null, value);
                        }
                        else
                        {
                            targetDic[name] = deep ? value.Clone(true) : value;
                        }
                        
                    }
                }
                index++;
            }
            return target;
        }
        /// <summary>
        /// Convert an object to JSON
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string ToJSON(object objectToSerialize)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (objectToSerialize is ExpandoObject)
            {
                return Flatten((ExpandoObject)objectToSerialize);
            }
            else
            {
                return (serializer.Serialize(objectToSerialize));
            }
            
        }
        /// <summary>
        /// Parse JSON into a typed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static T FromJSON<T>(string objectToDeserialize)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return (T)serializer.Deserialize(objectToDeserialize, typeof(T));
        }
        /// <summary>
        /// Parse JSON into an expando object
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static ExpandoObject FromJSON(string objectToDeserialize)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> dict = (Dictionary<string, object>)serializer.Deserialize(objectToDeserialize, typeof(Dictionary<string, object>));
            return Extend(true,null,dict);
        }

        protected static string Flatten(ExpandoObject expando)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StringBuilder sb = new StringBuilder();
            List<string> contents = new List<string>();
            var d = expando as IDictionary<string, object>;
            sb.Append("{");

            foreach (KeyValuePair<string, object> kvp in d)
            {
                contents.Add(String.Format("\"{0}\": {1}", kvp.Key,
                   serializer.Serialize(kvp.Value)));
            }
            sb.Append(String.Join(",", contents.ToArray()));

            sb.Append("}");

            return sb.ToString();
        }

#endregion
        /// <summary>
        /// Parses SIMPLE (non-nested) object
        /// TODO this needs a lot of work, theremust be something out there to parse json
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
        
        protected int GetElementIndex(IDomObject element) {
            int count = 0;
            foreach (IDomObject e in element.ParentNode.ChildNodes)
            {
                if (ReferenceEquals(e, element))
                {
                    break;
                }
                count++;
            }
            return count;
        }
        #region IEnumerable<IDomElement> Members

        public IEnumerator<IDomObject> GetEnumerator()
        {
            return Selection.GetEnumerator();
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
