using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.IO;
using System.Web.Script.Serialization;
using CsQuery.ExtensionMethods;
using CsQuery.Utility;
using CsQuery.Utility.StringScanner;
using CsQuery.Implementation;
using CsQuery.Engine;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery
{
    /// <summary>
    /// Document is an IDomRoot object, referred to sometimes as the "DOM", and represents the DOM that this 
    /// CsQuery objects applies to. When CQ methods are run, the resulting CQ object will refer to the same 
    /// Document as the original. Selectors always run against this DOM. 
    /// 
    /// Creating a CQ object from something that is not bound to a DOM (such as an HTML string, or an unbound
    /// IDomObject or IDomElement object) will result in a new Document being created, that is unrelated to any
    /// other active objects you may have. Adding unbound elements using methods such as Append will cause them
    /// to become part of the target DOM. They will be removed from whatever DOM they previously belonged to.
    /// (Elements cannot be part of more than one DOM). If you don't want to remove something while adding to
    /// a CQ object from a different DOM, then you should clone the elements.
    /// 
    /// Selection is a set of DOM nodes matching the selector. 
    /// 
    /// Elements is a set of IDomElement nodes matching the selector. This is a subset of Selection - it 
    /// excludes non-Element nodes.
    /// 
    /// The static Create() methods create new DOMs. To create a CsQuery object based on an existing dom, 
    /// use new CQ() (similar to jQuery() methods).
    /// </summary>
    
    public partial class CQ : IEnumerable<IDomObject>
    {
        // TODO:

        // End
        // WrapInner
        // OffsetParent

        // jquery.Contains
        // jquery.Grep
        
        // + some selectors
        

        /// <summary>
        /// Add the previous set of elements on the stack to the current set.
        /// </summary>
        /// <returns></returns>
        public CQ AndSelf()
        {
            var csq = new CQ(this);
            csq.Order = SelectionSetOrder.Ascending;
            
            if (CsQueryParent == null)
            {
                return csq;
            }
            else
            {
                csq.SelectionSet.AddRange(CsQueryParent.SelectionSet);
                return csq;
            }
        }
        /// <summary>
        /// End the most recent filtering operation in the current chain and return the set of matched elements 
        /// to its previous state
        /// </summary>
        /// <returns></returns>
        public CQ End()
        {
            return CsQueryParent ?? New();
        }

        /// <summary>
        /// The number of elements in the CsQuery object
        /// </summary>
        public int Length
        {
            get
            {
                return SelectionSet.Count;
            }
        }

        /// <summary>
        /// Return matched element. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IDomObject this[int index]
        {
            get {
                return Get(index);
            }
        }

        
        public CQ this[string selector]
        {
            get
            {
                return Select(selector);
            }
        }

        public CQ this[IDomObject element]
        {
            get
            {
                return Select(element);
            }
        }
        public CQ this[IEnumerable<IDomObject> element]
        {
            get
            {
                return Select(element);
            }
        }
        public IEnumerable<IDomObject> Get()
        {
            return SelectionSet;
        }
        public IDomObject Get(int index)
        {
            int effectiveIndex = index < 0 ? SelectionSet.Count+index-1 : index;
            return effectiveIndex >= 0 && effectiveIndex < SelectionSet.Count ?
                SelectionSet.ElementAt(effectiveIndex) :
                null;
        }
        /// <summary>
        /// Remove all child nodes of the set of matched elements from the DOM.
        /// </summary>
        /// <returns></returns>
        public CQ Empty()
        {
            
            return Each((IDomObject e) => {
                if (e.HasChildren)
                {
                    e.ChildNodes.Clear();
                }
            });
        }
        /// <summary>
        /// Set the HTML contents of each element in the set of matched elements. 
        /// Any elements without InnerHtml are ignored.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public CQ Html(params string[] html)
        {
            CQ htmlElements = EnsureCsQuery(mergeContent(html));
            bool first = true;

            foreach (DomElement obj in onlyElements(SelectionSet))
            {
                if (obj.InnerHtmlAllowed)
                {
                    obj.ChildNodes.Clear();
                    obj.ChildNodes.AddRange(first ? htmlElements : htmlElements.Clone());
                    first = false;
                }
            }
            return this;
        }
        /// <summary>
        /// Get the HTML contents of the first element in the set of matched elements.
        /// </summary>
        /// <returns></returns>
        public string Html()
        {
            return Length > 0 ? this[0].InnerHTML : String.Empty;
        }
        public CQ Not(string selector)
        {
            CQ csq = new CQ(SelectionSet);
            csq.SelectionSet.ExceptWith(Select(selector,this));
            csq.Selectors = Selectors;
            return csq;
        }
        public CQ Not(IDomObject element)
        {
            return Not(Objects.Enumerate(element));
        }
        public CQ Not(IEnumerable<IDomObject> elements)
        {
            CQ csq = new CQ(SelectionSet);
            csq.SelectionSet.ExceptWith(elements);
            csq.Selectors = Selectors;
            return csq;
        }
        /// <summary>
        /// Reduce the set of matched elements to those that have a descendant that matches the selector or DOM element.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ Has(string selector)
        {
            var csq = New();

            foreach (IDomObject obj in SelectionSet)
            {
                if (Select(obj).Find(selector).Length > 0)
                {
                    csq.SelectionSet.Add(obj);
                }
            }
            return csq;
        }
        public CQ Has(IDomObject element)
        {
            return Has(Objects.Enumerate(element));
        }
        public CQ Has(IEnumerable<IDomObject> elements)
        {
            var csq = New();
            foreach (IDomObject obj in SelectionSet)
            {
                if (obj.Cq().Find(elements).Length > 0)
                {
                    csq.SelectionSet.Add(obj);
                }
            }
            return csq;
        }
        /// <summary>
        /// Set the content of each element in the set of matched elements to the specified text.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CQ Text(string value)
        {
            foreach (IDomElement obj in Elements)
            {
                if (obj.InnerTextAllowed)
                {
                    obj.ChildNodes.Clear();
                    // Element types that cannot have HTML contents should not have the value encoded.
                    //string textValue = obj.InnerHtmlAllowed ? Objects.HtmlEncode(value) : value;
                    IDomText text = obj.InnerHtmlAllowed  ? new DomText(value) : new DomInnerText(value);
                    obj.ChildNodes.Add(text);
                }
            }
            return this;
        }
        public CQ Text(Func<object,object,object> func) {

            return this;
        }

        /// <summary>
        /// Get the combined text contents of each element in the set of matched elements, including their descendants.
        /// </summary>
        /// <returns></returns>
        public string Text()
        {
            StringBuilder sb = new StringBuilder();

            IDomObject lastElement = null;
            foreach (IDomObject obj in SelectionSet)
            {
                // Add a space between noncontiguous elements in the selection
                //if (lastElement != null && obj.Index > 0
                //    && obj.PreviousSibling != lastElement)
                //{
                //    sb.Append(" ");
                //}
                lastElement = obj;
                if (obj.NodeType == NodeType.TEXT_NODE)
                {
                    sb.Append(obj.NodeValue);
                }
                else
                {
                    Text(sb, obj.Cq().Contents());
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Helper for public Text() function to act recursively
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="elements"></param>
        protected void Text(StringBuilder sb, IEnumerable<IDomObject> elements)
        {
            IDomObject lastElement = null;
            foreach (IDomObject obj in elements)
            {
                if (lastElement != null && obj.Index > 0
                   && obj.PreviousSibling != lastElement)
                {
                    sb.Append(" ");
                }
                lastElement = obj;
                switch (obj.NodeType)
                {
                    case NodeType.TEXT_NODE:
                    case NodeType.CDATA_SECTION_NODE:
                    case NodeType.COMMENT_NODE:
                        sb.Append(obj.NodeValue);
                        break;
                    case NodeType.ELEMENT_NODE:
                        Text(sb, obj.ChildNodes);
                        break;
                }
            }
        }
        /// <summary>
        /// Add elements to the set of matched elements from a selector or an HTML fragment. Returns a new jQuery object.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public CQ Add(string selector)
        {
            return Add(Select(selector));
        }
        public CQ Add(IDomObject element)
        {
            return Add(Objects.Enumerate(element));
        }
        public CQ Add(IEnumerable<IDomObject> elements)
        {
            CQ res = new CQ(this);
            res.AddSelectionRange(elements);
            return res;
        }
        public CQ Add(string selector, IEnumerable<IDomObject> context)
        {
            return Add(Select(selector, context));
        }
        public CQ Add(string selector,IDomObject context)
        {
            return Add(Select(selector,context));
        }
        /// <summary>
        /// Adds the specified class(es) to each of the set of matched elements.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public CQ AddClass(string className)
        {
            foreach (var item in Elements)
            {
                item.AddClass(className);
            }
            return this;
        }
        /// <summary>
        /// Add or remove one or more classes from each element in the set of matched elements, 
        /// depending on either the class's presence.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public CQ ToggleClass(string classes)
        {
            IEnumerable<string> classList = classes.SplitClean(' ');
            foreach (IDomElement el in Elements) {
                foreach (string cls in classList)
                {
                    if (el.HasClass(cls))
                    {
                        el.RemoveClass(cls);
                    }
                    else
                    {
                        el.AddClass(cls);
                    }
                }
            }
            return this;
        }
        /// <summary>
        /// Add or remove one or more classes from each element in the set of matched elements, 
        /// depending on the value of the switch argument.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public CQ ToggleClass(string classes, bool addRemoveSwitch)
        {
            IEnumerable<string> classList = classes.SplitClean(' ');
            foreach (IDomElement el in Elements)
            {
                foreach (string cls in classList)
                {
                    if (addRemoveSwitch)
                    {
                        el.AddClass(cls); 
                    }
                    else
                    {
                        el.RemoveClass(cls);
                    }
                }
            }
            return this;
        }
        /// <summary>
        /// Determine whether any of the matched elements are assigned the given class.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public bool HasClass(string className)
        {
            
            IDomElement el = FirstElement();

            return el==null ? false :
                el.HasClass(className);
        }
        /// <summary>
        /// Insert content, specified by the parameter, to the end of each element in the set of matched elements.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public CQ Append(params string[] content)
        {
            return Append(mergeContent(content));
        }
        public CQ Append(IDomObject element)
        {
            return Append(Objects.Enumerate(element));
        }
        public CQ Append(IEnumerable<IDomObject> elements)
        {
            CQ ignoredOutput;
            return Append(elements, out ignoredOutput);
        }
        protected CQ Append(IEnumerable<IDomObject> elements, out CQ insertedElements)
        {
            insertedElements = New();
            bool first = true;
            foreach (var obj in Elements)
            {
                // Make sure they didn't really mean to add to a tbody or something
                IDomElement target = getTrueTarget(obj);

                // must copy the enumerable first, since this can cause
                // els to be removed from it
                List<IDomObject> list = new List<IDomObject>(elements);
                foreach (var e in list)
                {
                    IDomObject toInsert = first ? e : e.Clone();
                    target.AppendChild(toInsert);
                    insertedElements.SelectionSet.Add(toInsert);
                }
                first = false;
            }
            return this;
        }
        /// <summary>
        ///  Insert every element in the set of matched elements to the end of the target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public CQ AppendTo(string target)
        {
            return AppendTo(Select(target));

        }
        public CQ AppendTo(IDomObject target)
        {
            return AppendTo(Objects.Enumerate(target));
        }
        public CQ AppendTo(IEnumerable<IDomObject> targets)
        {
            CQ output;
            EnsureCsQuery(targets).Append(SelectionSet, out output);
            return output;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="func">
        /// delegate(int index, string html) 
        ///  A function that returns an HTML string to insert at the end of each element in the set of matched elements. 
        /// Receives the index position of the element in the set and the old HTML value of the element as arguments.
        /// </param>
        /// <returns></returns>
        public CQ Append(Func<int, string, string> func)
        {
            int index = 0;
            foreach (DomElement obj in Elements)
            {

                string val = func(index, obj.InnerHTML);
                obj.Cq().Append((string)val);
                index++;
            }
            return this;
        }
        public CQ Append(Func<int, string, IDomElement> func)
        {
            int index = 0;
            foreach (IDomElement obj in Elements)
            {
                IDomElement clientValue = func(index, obj.InnerHTML);
                obj.Cq().Append(clientValue);
                index++;
            }
            return this;
        }
        public CQ Append(Func<int, string, IEnumerable<IDomElement>> func)
        {
            int index = 0;
            foreach (IDomElement obj in Elements)
            {
                IEnumerable<IDomElement> val = func(index, obj.InnerHTML);
                obj.Cq().Append(val);
                index++;
            }
            return this;
        }
        /// <summary>
        /// Insert content, specified by the parameter, to the beginning of each element in the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ Prepend(params IDomObject[] element)
        {
            return Prepend(Objects.Enumerate(element));
        }

        public CQ Prepend(params string[] selector)
        {
            return Prepend(mergeContent(selector));
        }
       
        public CQ Prepend(IEnumerable<IDomObject> elements)
        {
            CQ ignoredOutput;
            return Prepend(elements, out ignoredOutput);
        }
        public CQ Prepend(IEnumerable<IDomObject> elements, out CQ insertedElements)
        {
            insertedElements = New();
            bool first = true;
            
            foreach (var target in Elements)
            {
                IEnumerable<IDomObject> content =
                    first ? elements : EnsureCsQuery(onlyElements(elements)).Clone().SelectionSet;


                int index = 0;
                foreach (var addedItem in content)
                {
                    target.ChildNodes.Insert(index++, addedItem);
                    insertedElements.SelectionSet.Add(addedItem);
                }
                first = false;
            }
            return this;
        }
        public CQ PrependTo(params string[] selector)
        {
            var target = New();
            Each(selector, item => target.SelectionSet.AddRange(Select(item)));
            target.Prepend(SelectionSet);
            return this;
        }
        public CQ PrependTo(params IDomObject[] element)
        {
            return PrependTo(element);
        }
        public CQ PrependTo(IEnumerable<IDomObject> targets)
        {
            CQ output;
            EnsureCsQuery(targets).Prepend(SelectionSet, out output);
            return output;
        }
        /// <summary>
        /// Get the value of an attribute for the first element in the set of matched elements.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Attr(string name)
        {
            name= name.ToLower();
            if (Length > 0)
            {
                string value;
                var el = this[0];
                switch(name) { 
                    case "class":
                        return el.ClassName;
                    case "style":
                        string st=  el.Style.ToString();
                        return st == "" ? null : st;
                    default:
                        if (el.TryGetAttribute(name, out value))
                        {
                            if (DomData.IsBoolean(name))
                            {
                                // Pre-1.6 and 1.6.1+ compatibility: always return the name of the attribute if it exists for
                                // boolean attributes
                                return name;
                            }
                            else
                            {
     
                                return value; 
                            }
                        } else if (name=="value" &&
                            (el.NodeName =="input" || el.NodeName=="select" || el.NodeName=="option")) {
                            return Val();
                        } else if (name=="value" && el.NodeName =="textarea") {
                            return el.InnerText;
                        }
                        break;
                }
                
            }
            return null;
        }
        /// <summary>
        /// Returns an attribute value as a nullable integer, or null if not an integer
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Attr<T>(string name)
        {
            string value;
            if (Length > 0 && this[0].TryGetAttribute(name, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return default(T);
        }
        /// <summary>
        /// Set one or more attributes for the set of matched elements.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CQ Attr(string name, IConvertible value)
        {

            // Make sure attempts to pass a JSON string end up a the right place
            if (Objects.IsJson(name) && value.GetType()==typeof(bool))
            {
                return AttrSet(name, (bool)value);
            }
            
            // jQuery 1.7 compatibility
            bool isBoolean = DomData.IsBoolean(name);
            if (isBoolean)
            {
                // Using attr with empty string should set a property to "true. But prop() itself requires a truthy value. Check for this specifically.
                if (value is string && (string)value == String.Empty)
                {
                    value = true;
                }
                SetProp(name, value);
                return this;
            }

            string val;
            if (value is bool)
            {
                val = value.ToString().ToLower();
            }
            else
            {
                val = GetValueString(value);
            }

            foreach (IDomElement e in Elements)
            {
                if ((e.NodeName == "input" || e.NodeName == "button") && name == "type"
                    && !e.IsDisconnected)
                {
                    throw new InvalidOperationException("Can't change type of \"input\" elements that have already been added to a DOM");
                }
                e.SetAttribute(name, val);
            }
            return this;
        }
        
        /// <summary>
        /// Map an object to attributes.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public CQ AttrSet(object attributes)
        {
            return AttrSet(attributes, false);
        }


        /// <summary>
        /// Map an object to attributes. If quickSet is true, treat give special treamtent to "css", "html", "text", "width" and "height" properties.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public CQ AttrSet(object attributes, bool quickSet=false)
        {
            IDictionary<string, object> dict;

            string cssString = attributes as string;
            if (cssString != null && Objects.IsJson((cssString)))
            {
                dict = ParseJSON<IDictionary<string, object>>(cssString);
            }
            else
            {
                dict = Objects.ToExpando(attributes);
            }

            foreach (IDomElement el in Elements)
            {
                foreach (var kvp in dict)
                {
                    if (quickSet)
                    {
                        string name = kvp.Key.ToLower();
                        switch (name)
                        {
                            case "css":
                                Select(el).CssSet(Objects.ToExpando(kvp.Value));
                                break;
                            case "html":
                                Select(el).Html(kvp.Value.ToString());
                                break;
                            case "height":
                            case "width":
                                // for height and width, do not set attributes - set css
                                Select(el).Css(name, kvp.Value.ToString());
                                break;
                            case "text":
                                Select(el).Text(kvp.Value.ToString());
                                break;
                            default:
                                el.SetAttribute(kvp.Key, kvp.Value.ToString());
                                break;
                        }
                    }
                    else
                    {
                        el.SetAttribute(kvp.Key, kvp.Value.ToString());
                    }
                }
            }
            return this;
        }
        /// <summary>
        /// Perform a substring replace on the contents of the named attribute in each item in the selection set. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="replaceWhat"></param>
        /// <param name="replaceWith"></param>
        /// <returns></returns>
        public CQ AttrReplace(string name, string replaceWhat, string replaceWith)
        {
            foreach (IDomElement item in SelectionSet)
            {
                string val = item[name];
                if (val != null)
                {
                    item[name] = val.Replace(replaceWhat, replaceWith);
                }
            }
            return this;
        }
        /// <summary>
        /// Remove an attribute from each element in the set of matched elements.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CQ RemoveAttr(string name)
        {
            foreach (IDomElement e in Elements)
            {
                switch (name)
                {
                    case "class":
                        e.ClassName = "";
                        break;
                    case "style":
                        e.Style.Clear();
                        break;
                    default:
                        e.RemoveAttribute(name);
                        break;
                }
            }
            return this;
        }
        /// <summary>
        ///  Remove a property for the set of matched elements.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CQ RemoveProp(string name)
        {
            return RemoveAttr(name);
        }
        /// <summary>
        /// Insert content, specified by the parameter, before each element in the set of matched elements.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public CQ Before(string selector)
        {
            return Before(Select(selector));
        }
        public CQ Before(IDomObject element)
        {
            return Before(Objects.Enumerate(element));
        }
        /// <summary>
        /// Insert content, specified by the parameter, before each element in the set of matched elements.
        /// </summary>
        public CQ Before(IEnumerable<IDomObject> selection)
        {
            EnsureCsQuery(selection).InsertAtOffset(SelectionSet, 0);
            return this;
        }
        /// <summary>
        ///  Insert content, specified by the parameter, after each element in the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ After(string selector)
        {
            return After(Select(selector));
        }
        /// <summary>
        ///  Insert content, specified by the parameter, after each element in the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ After(IDomObject element)
        {
            return After(Objects.Enumerate(element));
        }
        /// <summary>
        ///  Insert content, specified by the parameter, after each element in the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ After(IEnumerable<IDomObject> selection)
        {
            EnsureCsQuery(selection).InsertAtOffset(SelectionSet, 1);
            return this;
            
        }

        /// <summary>
        /// Remove the parents of the set of matched elements from the DOM, 
        /// leaving the matched elements in their place.
        /// </summary>
        /// <returns></returns>
        public CQ Unwrap()
        {
            HashSet<IDomObject> parents = new HashSet<IDomObject>();
            
            // Start with a unique list of parents instead of working with the siblings
            // to avoid repetition and unwrapping more than once for multiple siblings from
            // a single parent
            foreach (IDomObject obj in SelectionSet)
            {
                if (obj.ParentNode != null) {
                    parents.Add(obj.ParentNode);
                }
            }
            foreach (IDomObject obj in parents) {
                var csq = obj.Cq();
                csq.ReplaceWith(csq.Contents());

            }
            //Order = SelectionSetOrder.Ascending;
            return this;
        }
        public CQ Wrap(string wrappingSelector)
        {
            return Wrap(Select(wrappingSelector));
        }
        public CQ Wrap(IDomObject element)
        {
            return Wrap(Objects.Enumerate(element));
        }
        public CQ Wrap(IEnumerable<IDomObject> wrapper)
        {
            return Wrap(wrapper, false);
        }
        public CQ WrapAll(string wrappingSelector)
        {
            return WrapAll(Select(wrappingSelector));
        }
        public CQ WrapAll(IDomObject element)
        {
            return WrapAll(Objects.Enumerate(element));
        }
        public CQ WrapAll(IEnumerable<IDomObject> wrapper)
        {
            return Wrap(wrapper, true);
        }
        protected CQ Wrap(IEnumerable<IDomObject> wrapper, bool keepSiblingsTogether)
        {
            // get innermost structure
            CQ wrapperTemplate = EnsureCsQuery(wrapper);
            IDomElement wrappingEl= null;
            IDomElement wrappingElRoot=null;

            int depth = getInnermostContainer(wrapperTemplate.Elements, out wrappingEl, out wrappingElRoot);
          
            if (wrappingEl!=null) {
                IDomObject nextEl = null;
                IDomElement innerEl = null;
                IDomElement innerElRoot = null;
                foreach (IDomObject el in SelectionSet)
                {

                    if (nextEl==null 
                        || (!ReferenceEquals(nextEl,el)) && 
                            !keepSiblingsTogether)
                    {
                        var template = wrappingElRoot.Cq().Clone();
                        if (el.ParentNode != null)
                        {
                            template.InsertBefore(el);
                        } 
                        // This will always succceed because we tested before this loop. But we need
                        // to run it again b/c it's a clone now
                        getInnermostContainer(template.Elements, out innerEl, out innerElRoot);
                    }
                    nextEl = el.NextSibling;
                    innerEl.AppendChild(el);
                    
                }
            }
            return this;
        }
        /// <summary>
        /// Wrap an HTML structure around the content of each element in the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ WrapInner(string selector)
        {
            return WrapInner(Select(selector));
        }
        /// <summary>
        /// Wrap an HTML structure around the content of each element in the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ WrapInner(IDomObject wrapper)
        {
            return WrapInner(Objects.Enumerate(wrapper));
        }
        // <summary>
        /// Wrap an HTML structure around the content of each element in the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ WrapInner(IEnumerable<IDomObject> wrapper) {
            foreach (var el in Elements)
            {
                var self = el.Cq();
                var contents = self.Contents();
                if (contents.Length > 0)
                {
                    contents.WrapAll(wrapper);
                }
                else
                {
                    self.Append(wrapper);
                }
            }
            return this;
        }
        //protected 
        /// <summary>
        /// Get the children of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns></returns>
        public CQ Children(string filter=null)
        {
            return filterIfSelector(filter, SelectionChildren());
        }
        /// <summary>
        /// Description: Get the siblings of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns></returns>
        public CQ Siblings(string selector=null)
        {
            SelectionSet<IDomElement> siblings = new SelectionSet<IDomElement>();
            
            // Add siblings of each item in the selection except the item itself for that iteration.
            // If two siblings are in the selection set, then all children of their mutual parent should
            // be returned. Otherwise, all children except the item iteself.
            foreach (var item in SelectionSet)
            {
                foreach (var child in item.ParentNode.ChildElements) {
                    if (!ReferenceEquals(child,item))
                    {
                        siblings.Add(child);
                    }
                }
            }
            return filterIfSelector(selector,siblings, SelectionSetOrder.Ascending);
        }
        /// <summary>
        /// Create a deep copy of the set of matched elements.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public CQ Clone()
        {
            CQ csq = new CQ();
            
            foreach (IDomObject elm in SelectionSet)
            {
                IDomObject clone = elm.Clone();
                csq.Document.AppendChild(clone);
                csq.AddSelection(clone);
            }
            return csq;
        }
        /// <summary>
        /// Get the first ancestor element that matches the selector, beginning at the current element and progressing up through the DOM tree.
        /// </summary>
        /// <returns></returns>
        public CQ Closest(string selector)
        {
            CQ matchTo = Select(selector);
            return Closest(matchTo);
        }
        public CQ Closest(IDomObject element)
        {
            return Closest(Objects.Enumerate(element));
        }
        public CQ Closest(IEnumerable<IDomObject> elements)
        {
            // Use a hashset to operate faster - since we already haveone for the selection set anyway
            SelectionSet<IDomObject> selectionSet;
            if (elements is CQ)
            {
                selectionSet = ((CQ)elements).SelectionSet;
            }
            else
            {
                selectionSet = new SelectionSet<IDomObject>();
                selectionSet.AddRange(elements);
            }
            CQ csq = New();

            foreach (var el in SelectionSet)
            {
                var search = el;
                while (search != null)
                {
                    if (selectionSet.Contains(search))
                    {
                        csq.AddSelection(search);
                        return csq;
                    }
                    search = search.ParentNode;
                }

            }
            return csq;

        }
        /// <summary>
        /// Get the children of each element in the set of matched elements, including text and comment nodes.
        /// </summary>
        /// <returns></returns>
        public CQ Contents()
        {

            List<IDomObject> list = new List<IDomObject>();
            foreach (IDomObject obj in SelectionSet)
            {
                if (obj is IDomContainer)
                {
                    list.AddRange(obj.ChildNodes );
                }
            }

            return new CQ(list, this);
        }
        

        /// <summary>
        ///  Set one or more CSS properties for the set of matched elements from JSON data
        /// </summary>
        /// <param name="cssJson"></param>
        /// <returns></returns>
        public CQ CssSet(object css)
        {
            IDictionary<string, object> dict;
            if (Objects.IsJson(css))
            {
                dict = ParseJSON<IDictionary<string, object>>((string)css);
            }
            else
            {
                dict = Objects.ToExpando(css);
            }
            foreach (IDomElement e in Elements) 
            {
                foreach (var key in dict)
                {
                    e.Style[key.Key]= key.Value.ToString();
                }
            }
            return this;
        }

        /// <summary>
        ///  Set one or more CSS properties for the set of matched elements.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CQ Css(string name, IConvertible value)
        {
            string style = String.Empty;

            foreach (IDomElement e in Elements)
            {
                e.Style[name]=value.ToString();
            }
            return this;
        }

        /// <summary>
        /// Get the value of a style property for the first element in the set of matched elements,
        /// and converts to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="style"></param>
        /// <returns></returns>
        public T Css<T>(String style) where T: IConvertible 
        {
            IDomElement el =FirstElement();
            if (el==null) {
                return default(T);
            }

            IStringScanner scanner = Scanner.Create(el.Style[style] ?? "");
            T num;
            if (scanner.TryGetNumber<T>(out num))
            {
                return num;
            }
            else
            {
                return default(T);
            }
        }
        /// <summary>
        /// Get the value of a style property for the first element in the set of matched elements
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public string Css(string style)
        {
            IDomElement el = FirstElement();
            string def=null;
            if (el!=null) {
                def = el.Style[style];
                switch (style)
                {
                    case "display":
                        if (String.IsNullOrEmpty(def))
                        {
                            def = el.IsBlock ? "block" : "inline";
                        }
                        break;
                    case "opacity":
                        if (String.IsNullOrEmpty(def))
                        {
                            def = "1";
                        }
                        break;
                }
            }
            return def;
            
        }
        /// <summary>
        /// Returns all values at named data store for the first element in the jQuery collection, as set by data(name, value).
        /// (Any attributes starting with data-)
        /// </summary>
        /// <returns></returns>
        public IDynamicMetaObjectProvider Data()
        {
            JsObject data = new JsObject();
            IDomElement obj = FirstElement();
            if (obj != null)
            {
                foreach (var item in obj.Attributes)
                {
                    if (item.Key.StartsWith("data-"))
                    {
                        Extend(data,item.Value);
                    }
                }
            }
            return data;
        }
        /// <summary>
        /// Store arbitrary data associated with the specified element. Returns the value that was set.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public CQ Data(string key,string data)
        {
            foreach (IDomElement e in Elements)
            {
                e.SetAttribute("data-" + key, data);
            }
            return this;
        }
        /// <summary>
        /// Convert an object to JSON and store as data
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public CQ Data(string key, object data)
        {
            string json = CQ.ToJSON(data);
            foreach (IDomElement e in Elements)
            {
                e.SetAttribute("data-" + key, json);
            }
            return this;
        }
        /// <summary>
        /// Convert an object to JSON and stores each named property as a data element
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public CQ DataSet(object data)
        {
            JsObject obj = CQ.ToExpando(data);
            foreach (var kvp in obj)
            {
                Data(kvp.Key, kvp.Value);
            }
            return this;
        }
        /// <summary>
        /// Returns value at named data store for the first element in the jQuery collection, as set by data(name, value).
        /// </summary>
        public object Data(string element)
        {
            string data = First().Attr("data-" + element);
            
            return CQ.ParseJSON(data);
        }
        public T Data<T>(string key)
        {
            string data = First().Attr("data-" + key);
            return CQ.ParseJSON<T>(data);
        }
        /// <summary>
        /// Returns data as a string, with no attempt to decode it
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string DataRaw(string key)
        {
            return First().Attr("data-" + key);
        }
        /// <summary>
        /// Iterate over each matched element.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public CQ Each(Action<int, IDomObject> func)
        {
            int index = 0;
            foreach (IDomObject obj in Selection)
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

        public CQ Each(Action<IDomObject> func)
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
        public CQ Eq(int index)
        {
            if (index < 0)
            {
                index = Length + index-1;
            }
            if (index >= 0 && index < Length)
            {
                return new CQ(SelectionSet[index], this);
            }
            else
            {
                return New();
            }
        }

        
        /// <summary>
        /// Get the descendants of each element in the current set of matched elements, filtered by a selector, jQuery object, or element.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ Find(string selector)
        {
            CQ csq = New();
            Selectors = new SelectorChain(selector);
            csq.AddSelectionRange(Selectors.Select(Document, Children()));
            return csq;
        }
        public CQ Find(IEnumerable<IDomObject> elements)
        {
            CQ csq = New();
            Selectors = new SelectorChain(elements);
            csq.AddSelectionRange(Selectors.Select(Document, Children()));
            return csq;
        }
        public CQ Find(IDomObject element)
        {
            CQ csq =New();
            Selectors = new SelectorChain(element);
            csq.AddSelectionRange(Selectors.Select(Document, Children()));
            return csq;
        }

        public CQ Filter(string selector)
        {
            return new CQ(filterElements(SelectionSet, selector));

        }
        public CQ Filter(IDomObject element) {
            return Filter(Objects.Enumerate(element));
        }
        public CQ Filter(IEnumerable<IDomObject> elements) {
            CQ filtered = new CQ(this);
            filtered.SelectionSet.IntersectWith(elements);
            return filtered;            
        }
        public CQ Filter(Func<IDomObject, bool> function)
        {
            CQ result = New();
            foreach (IDomObject obj in SelectionSet)
            {
                if (function(obj)) {
                    result.AddSelection(obj);
                }
            }
            return result;
        }
        public CQ Filter(Func<IDomObject, int, bool> function)
        {
            CQ result = New();
            int index = 0;
            foreach (IDomObject obj in SelectionSet)
            {
                if (function(obj,index++))
                {
                    result.AddSelection(obj);
                }
            }
            return result;
        }

        /// <summary>
        /// Select elements and return a new CSQuery object 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ Select(string selector)
        {
            CQ csq = New();
            csq.Selectors = new SelectorChain(selector);
            // If the selector is HTML create it as a new fragment so it can be indexed & traversed upon
            //IDomRoot dom = selectors.IsHtml ? new DomFragment(selector.ToCharArray()) : Document;
            csq.AddSelectionRange(csq.Selectors.Select(Document));
            return csq;
        }

        public CQ Select(IDomObject element)
        {
            CQ csq = new CQ(element,this);
            return csq;
        }
        public CQ Select(IEnumerable<IDomObject> elements)
        {
            CQ csq = new CQ(elements,this);
            return csq;
        }
        /// <summary>
        /// Select elements from within a context
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public CQ Select(string selector, IDomObject context)
        {
            var selectors = new SelectorChain(selector);
            CQ csq = new CQ(selectors.Select(Document, context), this);
            csq.Selectors = selectors;
            return csq;
        }
        public CQ Select(string selector, IEnumerable<IDomObject> context)
        {
            var selectors = new SelectorChain(selector);
            CQ csq = new CQ(selectors.Select(Document, context), this);
            csq.Selectors = selectors;
            return csq;
        }
        /// <summary>
        /// Reduce the set of matched elements to the first in the set.
        /// </summary>
        /// <returns></returns>
        public CQ First()
        {
            return Eq(0);
        }
        /// <summary>
        /// Reduce the set of matched elements to the last in the set.
        /// </summary>
        /// <returns></returns>
        public CQ Last()
        {
            if (SelectionSet.Count == 0)
            {
                return New();
            }
            else
            {
                return Eq(SelectionSet.Count - 1);
            }
        }
        /// <summary>
        /// Hide the matched elements.
        /// </summary>
        /// <returns></returns>
        public CQ Hide()
        {
            foreach (IDomElement e in Elements)
            {
                e.Style["display"]= "none";
            }
            return this;

        }
        /// <summary>
        /// Toggle the visiblity state of the matched elements.
        /// </summary>
        /// <returns></returns>
        public CQ Toggle()
        {
           foreach (IDomElement e in Elements)
            {
                string displ = e.Style["display"];
                bool isVisible = displ == null || displ != "none";
                e.Style["display"] = isVisible ? "none" : null;
            }
           return this;
        }
        /// <summary>
        /// Display or hide the matched elements.
        /// </summary>
        /// <returns></returns>
        public CQ Toggle(bool isVisible)
        {
            foreach (IDomElement e in Elements)
            {
                if (isVisible)
                {
                    e.RemoveStyle("display");
                }
                else
                {
                    e.Style["display"] = "none";
                }
            }
            return this;
        }
        /// <summary>
        /// Search for a given element from among the matched elements.
        /// </summary>
        /// <returns></returns>
        public int Index()
        {
            IDomObject el = SelectionSet.FirstOrDefault();
            if (el != null)
            {
                return GetElementIndex(el);
            }
            return -1;
        }
        /// <summary>
        /// Returns the position of the current selection within the new selection defined by "selector"
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public int Index(string selector)
        {
            var selection = Select(selector);
            return selection.Index(SelectionSet);
        }
        public int Index(IDomObject elements)
        {
            return Index(Objects.Enumerate(elements));
        }
        public int Index(IEnumerable<IDomObject> elements)
        {
            IDomObject find = elements.FirstOrDefault();
            int index = -1;
            if (find != null)
            {
                int count = 0;
                foreach (IDomObject el in SelectionSet)
                {
                    if (ReferenceEquals(el, find))
                    {
                        index=count;
                        break;
                    }
                    count++;
                }
            }
            return index;
        }

        /// <summary>
        /// Insert every element in the set of matched elements after the target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public CQ InsertAfter(IDomObject target)
        {
            return InsertAtOffset(target,1);
        }
        /// <summary>
        /// Insert every element in the set of matched elements after the target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public CQ InsertAfter(IEnumerable<IDomObject> target) {
            return InsertAtOffset(target, 1);
        }
        /// <summary>
        /// Insert every element in the set of matched elements after the target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public CQ InsertAfter(string target)
        {
            return InsertAfter(Select(target));
        }
        /// <summary>
        /// Support for InsertAfter and InsertBefore. An offset of 0 will insert before the current element. 1 after.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected CQ InsertAtOffset(IDomObject target, int offset)
        {
            int index = target.Index;
            
            foreach (var item in SelectionSet)
            {
                target.ParentNode.ChildNodes.Insert(index+offset,item);
                index++;
            }
            return this;
        }

        /// <summary>
        /// A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted before the element(s) specified by this parameter.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public CQ InsertBefore(string selector)
        {
            return InsertBefore(Select(selector));
        }
        public CQ InsertBefore(IDomObject target)
        {
            return InsertAtOffset(target, 0);
        }
        public CQ InsertBefore(IEnumerable<IDomObject> target)
        {
            return InsertAtOffset(target, 0);
        }

        /// <summary>
        /// Get the immediately preceding sibling of each element in the set of matched elements, 
        /// optionally filtered by a selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ Prev(string selector=null)
        {
            return nextPrevImpl(selector, false);
        }   

        /// <summary>
        /// Get the immediately following sibling of each element in the set of matched elements. 
        /// If a selector is provided, it retrieves the next sibling only if it matches that selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ Next(string selector=null)
        {
            return nextPrevImpl(selector, true);
        }

        /// <summary>
        /// Get all following siblings of each element in the set of matched elements, 
        /// optionally filtered by a selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ NextAll(string filter = null)
        {
            return nextPrevAllImpl(filter, true);
        }

        /// <summary>
        /// Get all following siblings of each element up to but not including the element matched by the selector, DOM node, or jQuery object passed
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public CQ NextUntil(string selector=null, string filter=null)
        {
            return nextPrevUntilImpl(selector, filter,true);
        }

        /// <summary>
        /// Get all following siblings of each element in the set of matched elements, 
        /// optionally filtered by a selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ PrevAll(string filter = null)
        {
            return nextPrevAllImpl(filter, false);
        }
        /// <summary>
        /// Get all preceding siblings of each element up to but not including the element matched by the selector, DOM node, or jQuery object passed
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public CQ PrevUntil(string selector=null, string filter=null)
        {
            return nextPrevUntilImpl(selector, filter, false);
        }
        protected CQ nextPrevImpl(string selector, bool next)
        {
            return filterIfSelector(selector,
                ForEach(Elements, (input) =>
                {
                    return next ? input.NextElementSibling : input.PreviousElementSibling;
                }), next ? SelectionSetOrder.Ascending:SelectionSetOrder.Descending);
        }
        protected CQ nextPrevAllImpl(string filter, bool next)
        {
            return filterIfSelector(filter, ForEachMany(Elements, (input) =>
            {
                return nextPrevAllImpl(input, next);
            }),next ? SelectionSetOrder.Ascending:SelectionSetOrder.Descending);
        }
        protected CQ nextPrevUntilImpl(string selector, string filter, bool next)
        {
            if (string.IsNullOrEmpty(selector))
            {
                return next ? NextAll(filter) : PrevAll(filter);
            }

            HashSet<IDomElement> untilEls = new HashSet<IDomElement>(Select(selector).Elements);
            return filterIfSelector(filter, ForEachMany(Elements, (input) =>
            {
                return nextPrevUntilFilterImpl(input, untilEls, next);
            }),next ? SelectionSetOrder.Ascending:SelectionSetOrder.Descending);
        }
        protected IEnumerable<IDomObject> nextPrevAllImpl(IDomObject input, bool next)
        {
            IDomObject item = next ? input.NextElementSibling : input.PreviousElementSibling;
            while (item != null)
            {
                yield return item;
                item = next ? item.NextElementSibling : item.PreviousElementSibling;
            }
        }
        protected IEnumerable<IDomObject> nextPrevUntilFilterImpl(IDomObject input, HashSet<IDomElement> untilEls, bool next)
        {
            foreach (IDomElement el in nextPrevAllImpl(input,next))
            {
                if (untilEls.Contains(el))
                {
                    break;
                }
                yield return el;
            }
        }
        /// <summary>
        /// Reduce the set of matched elements to a subset beginning with the index provided
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public CQ Slice(int start)
        {
            return Slice(start, SelectionSet.Count);
        }
        /// <summary>
        /// Reduce the set of matched elements to a subset specified by a range of indices.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public CQ Slice(int start, int end)
        {
            if (start < 0)
            {
                start = SelectionSet.Count + start;
                if (start < 0) { start = 0; }
            }
            if (end < 0)
            {
                end = SelectionSet.Count + end;
                if (end < 0) { end = 0; }
            }
            if (end >= SelectionSet.Count)
            {
                end = SelectionSet.Count - 1;
            }

            CQ output= New();
            
            for (int i = start; i < end; i++)
            {
                output.SelectionSet.Add(SelectionSet[i]);
            }
            
            return output;
        }

        /// <summary>
        /// Get the parent of each element in the current set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ Parent(string selector=null)
        {
            return filterIfSelector(selector, ForEachMany(Elements, parentImpl));
        }
        protected IEnumerable<IDomObject> parentImpl(IDomObject input)
        {
            if (input.ParentNode != null &&
                input.ParentNode.NodeType == NodeType.ELEMENT_NODE)
            {
                yield return input.ParentNode;
            }
        }
        /// <summary>
        ///  Get the ancestors of each element in the current set of matched elements, 
        ///  optionally filtered by a selector.
        /// </summary>
        /// <returns></returns>
        public CQ Parents(string filter=null)
        {
            return ParentsUntil(null, filter);
        }
        public CQ ParentsUntil(string selector=null, string filter=null)
        {
            HashSet<IDomElement> match = new HashSet<IDomElement>();
            if (selector != null)
            {
                match.AddRange(Select(selector).Elements);
            }

            CQ output = New();
            output.SelectionSet.AddRange(filterElementsIgnoreNull(parentsImpl(Elements, match),filter));
            return output;
        }
       
        protected IEnumerable<IDomElement> parentsImpl(IEnumerable<IDomElement> source, HashSet<IDomElement> until)
        {

            HashSet<Tuple<int, int, IDomElement>> results = new HashSet<Tuple<int, int, IDomElement>>();

            int index=0;
            foreach (var item in source)
            {
                int depth = item.Depth;
                var parent =item.ParentNode;
                while (parent is IDomElement && !until.Contains(parent))
                {
                    results.Add(new Tuple<int, int, IDomElement>(depth--, index++, (IDomElement)parent));
                    parent = parent.ParentNode;
                }
            }
            var comp = new parentComparer();
            return results.OrderBy(item=>item,comp).Select(item => item.Item3);
        }
        class parentComparer : IComparer<Tuple<int, int, IDomElement>>
        {

            public int Compare(Tuple<int, int, IDomElement> x, Tuple<int, int, IDomElement> y)
            {
                int depth = y.Item1 - x.Item1;
                return depth != 0 ? depth : x.Item2 - y.Item2;
            }
        }
        /// <summary>
        /// Set one or more properties for the set of matched elements.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public CQ Prop(string name, IConvertible value)
        {
            // Prop actually works on things other than boolean - e.g. SelectedIndex. For now though only use prop for booleans

            if (DomData.IsBoolean(name))
            {
                SetProp(name, value);
            }
            else
            {
                Attr(name, value);
            }
            return this;
        }
        public bool Prop(string name)
        {
            name=name.ToLower();
            if (Length>0 && DomData.IsBoolean(name)) {
                bool has = this[0].HasAttribute(name);
                // if there is nothing with the "selected" attribute, in non-multiple select lists, 
                // the first one is selected by default by Sizzle. We will return that same information 
                // when using prop.
                // TODO: this won't work for the "selected" selector. Need to move this logic into DomElement 
                // and use selected property instead to make this work. I am not sure I agree with the jQuery
                // implementation anyway since querySelectorAll does NOT return this
                if (name == "selected" && !has)
                {
                    var owner = First().Closest("select");
                    string ownerSelected = owner.Val();
                    if (ownerSelected == String.Empty && !owner.Prop("multiple"))
                    {
                        return ReferenceEquals(owner.Find("option")[0], this[0]);
                    }

                }
                return has;
            }
            return false;
        }


        /// <summary>
        /// Remove all selected elements from the DOM
        /// </summary>
        /// <returns></returns>
        public CQ Remove(string selector=null)
        {
            SelectionSet<IDomObject> list = !String.IsNullOrEmpty(selector) ?
                Filter(selector).SelectionSet :
                SelectionSet;
            
            // We need to copy first because selection can change
            List<IDomObject> removeList = new List<IDomObject>(list);
            List<bool> disconnected = list.Select(item => item.IsDisconnected).ToList();

            for (int index=0;index<list.Count;index++) 
            {
                var el = removeList[index];
                if (disconnected[index])
                {
                    list.Remove(el);
                }
                if (el.ParentNode!=null) {
                    el.Remove();
                }
            }
            return this;
        }
        /// <summary>
        /// This is synonymous with Remove in CsQuery, since there's nothing associated with an element
        /// that is not rendered.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ Detach(string selector = null)
        {
            return Remove(selector);
        }
        
        /// <summary>
        /// Remove a single class, multiple classes, or all classes from each element in the set of matched elements.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public CQ RemoveClass(string className=null)
        {
            
           foreach (IDomElement e in Elements)
            {
                if (!String.IsNullOrEmpty(className))
                {
                    e.RemoveClass(className);
                }
                else
                {
                    e.ClassName = null;
                }
            }
           return this;
        }

        /// <summary>
        /// Remove a previously-stored piece of data.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public CQ RemoveData(string dataId=null)
        {
            foreach (IDomElement el in Elements)
            {
                List<string> toRemove = new List<string>();
                foreach (var kvp in el.Attributes)
                {
                    bool match = String.IsNullOrEmpty(dataId) ?
                        kvp.Key.StartsWith("data-") :
                        kvp.Key == "data-" + dataId;
                    if (match) 
                    {
                        toRemove.Add(kvp.Key);
                    }
                }
                foreach (string key in toRemove)
                {
                    el.Attributes.Remove(key);
                }
            }
            return this;
        }
        /// <summary>
        /// Determine whether an element has any jQuery data associated with it.
        /// </summary>
        /// <returns></returns>
        public bool HasData()
        {
            foreach (IDomElement el in Elements)
            {
                foreach (var kvp in el.Attributes)
                {
                    if (kvp.Key.StartsWith("data-"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Replace each element in the set of matched elements with the provided new content.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ ReplaceWith(params string[] content)
        {
            if (Length > 0)
            {
                // Before allows adding of new content to an empty selector. To ensure consistency with jQuery
                // implentation, do not do this if called on an empty selector. 

                // The logic here is tricky because we can do a replace on disconnected selection sets. This has to
                // track what was orignally scheduled for removal in case the set changes in "Before" b/c it's disconnected.

                CQ newContent = EnsureCsQuery(mergeContent(content));
                CQ replacing = new CQ(this);
                
                Before(newContent);
                SelectionSet.ExceptWith(replacing);
                replacing.Remove();                
                return this;
            }
            else
            {
                return this;
            }
        }
        public CQ ReplaceWith(IDomObject element)
        {
            return ReplaceWith(Objects.Enumerate(element));
        }
        public CQ ReplaceWith(IEnumerable<IDomObject> elements)
        {
            return Before(elements).Remove();
        }
        /// <summary>
        /// Replace the target element with the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ ReplaceAll(string selector)
        {
            return ReplaceAll(Select(selector));
        }
        /// <summary>
        /// Replace the target element with the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ ReplaceAll(IDomObject target)
        {
            return ReplaceAll(Objects.Enumerate(target));
        }
        /// <summary>
        /// Replace each target element with the set of matched elements.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public CQ ReplaceAll(IEnumerable<IDomObject> targets)
        {
            return EnsureCsQuery(targets).ReplaceWith(SelectionSet);
        }
        public CQ Show()
        {
            foreach (IDomElement e in Elements)
            {
                e.RemoveStyle("display");
            }
            return this;
        }
        // Not used yet, will be for visible selector
        // Also not correct
        //protected bool IsVisible()
        //{
        //    bool parentHidden = false;
        //    IDomObject el = e.ParentNode;
        //    while (el != null)
        //    {
        //        string st = el.Style["display"];
        //        if (st == "none")
        //        {
        //            parentHidden = true;
        //            break;
        //        }
        //        el = el.ParentNode;
        //    }
        //    return parentHidden;
        //}
        /// <summary>
        /// Get the current value of the first element in the set of matched elements, and try to convert to the specified type
        /// </summary>
        /// <returns></returns>
        public T Val<T>()
        {
            string val = Val();
            return Objects.Convert<T>(val);
        }
        public T ValOrDefault<T>()
        {
            string val = Val();
            T outVal;
            if (Objects.TryConvert<T>(val, out outVal))
            {
                return outVal;
            }
            else
            {
                return (T)Objects.DefaultValue(typeof(T));
            }
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
                        return e.InnerText;
                    case "input":
                        string val = e.GetAttribute("value",String.Empty);
                        switch(e.GetAttribute("type",String.Empty)) {
                            case "radio":
                            case "checkbox":
                                if (String.IsNullOrEmpty(val))
                                {
                                    val = "on";
                                }
                                break;
                            default:
                                break;
                        }
                        return val;
                    case "select":
                        string result = String.Empty;
                        // TODO optgroup handling (just like the setter code)
                        var options =Find("option");
                        if (options.Length==0) {
                            return null;
                        }
                        
                        foreach (IDomElement child in options)
                        {
                            bool disabled = child.HasAttribute("disabled") || (child.ParentNode.NodeName == "optgroup" && child.ParentNode.HasAttribute("disabled"));

                            if (child.HasAttribute("selected") && !disabled)
                            {
                                var optVal = child.GetAttribute("value");
                                if (optVal == null)
                                {
                                    optVal = child.Cq().Text();
                                }
                                result = result.ListAdd(optVal,",");
                                if (!e.HasAttribute("multiple"))
                                {
                                    break;
                                }
                            }
                        }
                        
                        if (result == String.Empty)
                        {
                            result = options[0].GetAttribute("value", String.Empty);
                        }
                        return result;
                    case "option":
                        val = e.GetAttribute("value");
                        return val ?? e.InnerText;
                    default:
                        return e.GetAttribute("value",String.Empty);
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Set the value of each element in the set of matched elements. If a comma-separated value is passed to a multuple select list, then it
        /// will be treated as an array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CQ Val(object value)
        {
            bool first = true;
            string val = GetValueString(value);
            foreach (IDomElement e in Elements)
            {
                switch (e.NodeName)
                {
                    case "textarea":
                        // should we delete existing children first? they should not exist
                        e.InnerText = val;
                        break;
                    case "input":
                        switch (e.GetAttribute("type",String.Empty))
                        {
                            case "checkbox":
                            case "radio":
                                if (first)
                                {
                                    SetOptionSelected(Elements, value, true);
                                }
                                break;
                            default:
                                e.SetAttribute("value", val);
                                break;
                        }
                        break;
                    case "select":
                        if (first) {
                            var multiple = e.HasAttribute("multiple");
                            SetOptionSelected(e.ChildElements, value, multiple);
                        }
                        break;
                    default:
                        e.SetAttribute("value", val);
                        break;
                }
                first = false;

            }
            return this;
        }
 
        

        /// <summary>
        /// Set the value of each mutiple select element in the set of matched elements. Any elements not of type &lt;SELECT multiple&gt;&lt;/SELECT&gt; will be ignored.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //public CsQuery Val(IEnumerable<object> values) {
        //    string valueString=String.Empty;
        //    foreach (object val in values) {
        //        valueString+=(String.IsNullOrEmpty(val.ToString())?String.Empty:",") + val.ToString();
        //    }
        //    foreach (IDomElement e in Elements)
        //    {
        //        if (e.NodeName == "select" && e.HasAttribute("multiple"))
        //        {
        //            Val(valueString);
        //        }
        //    }
        //    return this;
        //}
        /// <summary>
        /// Set the CSS width of each element in the set of matched elements.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CQ Width(int value)
        {
            return Width(value.ToString() + "px");
        }
        public CQ Width(string value)
        {
            return Css("width", value);
        }
        /// <summary>
        /// Set the CSS width of each element in the set of matched elements.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CQ Height(int value)
        {
            return Height(value.ToString() + "px");
        }
        public CQ Height(string value)
        {
            return Css("height", value);
        }

        /// <summary>
        /// Check the current matched set of elements against a selector and return true if at least one of these elements matches the selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public bool Is(string selector)
        {
            return Filter(selector).Length > 0;
        }
        public bool Is(IEnumerable<IDomObject> elements)
        {
            HashSet<IDomObject> els = new HashSet<IDomObject>(elements);
            els.IntersectWith(SelectionSet);
            return els.Count > 0;
        }
        public bool Is(IDomObject element)
        {
            return SelectionSet.Contains(element);
        }

      

    }


    
}
