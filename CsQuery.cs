using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Jtc.ExtensionMethods;
using Jtc.Scripting;
using System.Diagnostics;

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
    
    public class CsQuery : IEnumerable<IDomElement>
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
            Dom.AddRange(ElementFactory.CreateObjects(html));
            AddSelectionRange(Dom.Children);
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
            Dom.AddRange(elements);
            AddSelectionRange(Dom.Children);
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

            if (selector == null)
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
            DomOwner = elements.First().Owner;
            
            //if (!(this is CsQuery))
            //{
                AddSelectionRange(elements);
            //}
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
            CsQuery csq = new CsQuery();
            
            csq.CreateNew(html);

            return csq;
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
            if (!ReferenceEquals(element.Dom, Dom))
            {
                throw new Exception("Cannot add unbound elements or elements bound to another DOM directly to a selection set.");
            }
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

        protected IEnumerable<IDomElement> SelectedElements
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
                if (_Selection[index] is IDomElement)
                {
                    return (IDomElement)_Selection[index];
                }
                else
                {
                    return null;
                }
            }
        }

        
        public CsQuery this[string selector]
        {
            get
            {
                return Find(selector);
            }
        }
        public IDomObject Get(int index)
        {
            return (IDomObject)_Selection[index];

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
            foreach (var item in this)
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
                IEnumerable<IDomObject> objects = ElementFactory.CreateObjects(clientValue);
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
            foreach (IDomObject element in this)
            {
                int index = GetElementIndex(element);
                foreach (IDomObject obj in selection)
                {
                    element.Parent.Insert( obj,index);
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
            foreach (IDomElement obj in SelectedElements)
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
            
            foreach (IDomObject elm in SelectedElements)
            {
                IDomObject clone = elm.Clone();
                csq.Dom.Add(clone);
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

            foreach (IDomElement e in SelectedElements)
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
        public CsQuery Each(Action<int, IDomObject> func)
        {
            int index = 0;
            foreach (IDomObject obj in Selection)
            {
                func(index, obj);
            }
            return this;
        }
        public CsQuery Each(Action<int, IDomElement> func)
        {
            int index = 0;
            foreach (IDomElement obj in Selection)
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
            foreach (IDomElement obj in SelectedElements)
            {
                func(obj);
            }
            return this;
        }
        public CsQuery Each(Action<IDomObject> func)
        {
            foreach (IDomObject obj in Selection)
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
            int index = GetElementIndex(target);
            foreach (var item in this)
            {
                target.Parent.Insert(item, ++index);
            }
            target.Parent.Insert(target,GetElementIndex(target)+1);
            return this;
        }
        public CsQuery InsertAfter(CsQuery target) {
            bool isFirst = true;
            foreach (IDomElement e in target)
            {
                if (isFirst)
                {
                    InsertAfter(e);
                    isFirst = false;
                }
                else
                {
                    e.Clone();
                    IDomObject clone = e.Clone();
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
            List<IDomElement> list = new List<IDomElement>();
            foreach (IDomElement obj in SelectedElements)
            {
                // Extraordinarily inefficient way to get next. TODO: make structure a linked list
                
                IDomElement next=null;
                var children = obj.Parent.Children.GetEnumerator();
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

            foreach (IDomElement obj in SelectedElements)
            {
                if (obj.Parent is IDomElement)
                {
                    list.Add((IDomElement)obj.Parent);
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
            foreach (IDomElement obj in SelectedElements)
            {
                // Extraordinarily inefficient way to get next. TODO: make structure a linked list
                
                var children = obj.Parent.Children.GetEnumerator();
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
            foreach (IDomElement e in selectors.GetMatches(SelectedElements))
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
            foreach (var elm in this)
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
            foreach (IDomElement e in SelectedElements)
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
                IDomElement e = this[0];
                switch(e.Tag) {
                    case "textarea":
                        return e.InnerHtml;
                    case "input":
                        switch(e.GetAttribute("type",String.Empty)) {
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
            foreach (IDomElement e in SelectedElements)
            {
                switch (e.Tag)
                {
                    case "textarea":
                        // should we delete existing children first? they should not exist
                        e.RemoveChildren();
                        DomText lit = new DomText(value);
                        e.Add(lit);
                        break;
                    case "input":
                        switch (e.GetAttribute("type",String.Empty))
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
        
        protected int GetElementIndex(IDomObject element) {
            int count = 0;
            foreach (IDomObject e in element.Parent.Children)
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

        public IEnumerator<IDomElement> GetEnumerator()
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
