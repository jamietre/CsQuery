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
using CsQuery.HtmlParser;
using CsQuery.StringScanner;
using CsQuery.Implementation;
using CsQuery.Engine;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;

namespace CsQuery
{
    /// <summary>
    /// The CQ object is analogus to the basic jQuery object. It has instance methods that mirror the
    /// methods of a jQuery object, and static methods that mirror utility methods such as "$.map".
    /// 
    /// Most methods return a new jQuery object that is bound to the same document, but a different
    /// selection set. In a web browser, you genally only have a single context (the browser DOM).
    /// Here, you could have many, though most of the time you will only be working with one.
    /// </summary>
    ///
    /// <remarks>
    /// Document is an IDomDocument object, referred to sometimes as the "DOM", and represents the
    /// DOM that this CsQuery objects applies to. When CQ methods are run, the resulting CQ object
    /// will refer to the same Document as the original. Selectors always run against this DOM.
    /// 
    /// Creating a CQ object from something that is not bound to a DOM (such as an HTML string, or an
    /// unbound IDomObject or IDomElement object) will result in a new Document being created, that
    /// is unrelated to any other active objects you may have. Adding unbound elements using methods
    /// such as Append will cause them to become part of the target DOM. They will be removed from
    /// whatever DOM they previously belonged to. (Elements cannot be part of more than one DOM). If
    /// you don't want to remove something while adding to a CQ object from a different DOM, then you
    /// should clone the elements.
    /// 
    /// Selection is a set of DOM nodes matching the selector.
    /// 
    /// Elements is a set of IDomElement nodes matching the selector. This is a subset of Selection -
    /// it excludes non-Element nodes.
    /// 
    /// The static Create() methods create new DOMs. To create a CsQuery object based on an existing
    /// dom, use new CQ() (similar to jQuery() methods).
    /// </remarks>

    public partial class CQ : IEnumerable<IDomObject>
    {
        #region public properties

        /// <summary>
        /// The number of elements in the CQ object.
        /// </summary>
        ///
        /// <url>
        /// http://api.jquery.com/length/
        /// </url>

        public int Length
        {
            get
            {
                return SelectionSet.Count;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Add the previous set of elements on the stack to the current set.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/andself/
        /// </url>

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
        /// End the most recent filtering operation in the current chain and return the set of matched
        /// elements to its previous state.
        /// </summary>
        ///
        /// <returns>
        /// The CQ object at the root of the current chain, or a new, empty selection if this CQ object
        /// is the direct result of a Create()
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/end/
        /// </url>

        public CQ End()
        {
            return CsQueryParent ?? New();
        }

        /// <summary>
        /// Return the active selection set.
        /// </summary>
        ///
        /// <returns>
        /// An sequence of IDomObject elements representing the current selection set.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/get/
        /// </url>

        public IEnumerable<IDomObject> Get()
        {
            return SelectionSet;
        }

        /// <summary>
        /// Return a specific element from the selection set.
        /// </summary>
        ///
        /// <param name="index">
        /// The zero-based index of the element to be returned.
        /// </param>
        ///
        /// <returns>
        /// An IDomObject.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/get/
        /// </url>
        
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
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/empty/
        /// </url>

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
        /// Get the HTML contents of the first element in the set of matched elements.
        /// </summary>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/html/#html1
        /// </url>

        public string Html()
        {
            return Length > 0 ? this[0].InnerHTML : String.Empty;
        }

        /// <summary>
        /// Set the HTML contents of each element in the set of matched elements. Any elements without
        /// InnerHtml are ignored.
        /// </summary>
        ///
        /// <param name="html">
        /// One or more strings of HTML markup.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/html/#html2
        /// </url>

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
        /// Selects all elements that do not match the given selector.
        /// </summary>
        ///
        /// <param name="selector">
        /// A CSS selector.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/not/
        /// </url>

        public CQ Not(string selector)
        {
            var notSelector = new Selector(selector);
            return new CQ(notSelector.Except(Document, SelectionSet));
        }

        /// <summary>
        /// Selects all elements except the element passed as a parameter.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to exclude.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/not/
        /// </url>

        public CQ Not(IDomObject element)
        {
            return Not(Objects.Enumerate(element));
        }

        /// <summary>
        /// Selects all elements except those passed as a parameter.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to be excluded.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/not/
        /// </url>

        public CQ Not(IEnumerable<IDomObject> elements)
        {
            CQ csq = new CQ(SelectionSet);
            csq.SelectionSet.ExceptWith(elements);
            csq.Selectors = Selectors;
            return csq;
        }

        /// <summary>
        /// Reduce the set of matched elements to those that have a descendant that matches the selector
        /// or DOM element.
        /// </summary>
        ///
        /// <param name="selector">
        /// A valid CSS/jQuery selector.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/has/
        /// </url>

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

        /// <summary>
        /// Reduce the set of matched elements to those that have the element passed as a descendant.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to match.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/has/
        /// </url>

        public CQ Has(IDomObject element)
        {
            return Has(Objects.Enumerate(element));
        }

        /// <summary>
        /// Reduce the set of matched elements to those that have each of the elements passed as a descendant.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to be excluded.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/has/
        /// </url>

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
        /// Get the combined text contents of each element in the set of matched elements, including
        /// their descendants.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the text contents of the selection.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/text/#text1
        /// </url>

        public string Text()
        {
            StringBuilder sb = new StringBuilder();

            Text(sb, SelectionSet);
            
            return sb.ToString();
        }

        /// <summary>
        /// Set the content of each element in the set of matched elements to the specified text.
        /// </summary>
        ///
        /// <param name="value">
        /// A string of text.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/text/#text2
        /// </url>

        public CQ Text(string value)
        {
            foreach (IDomElement obj in Elements)
            {
                SetChildText(obj, value);
            }
            return this;
        }

        /// <summary>
        /// Set the content of each element in the set of matched elements to the text returned by the
        /// specified function delegate.
        /// </summary>
        ///
        /// <param name="func">
        /// A delegate to a function that returns an HTML string to insert at the end of each element in
        /// the set of matched elements. Receives the index position of the element in the set and the
        /// old HTML value of the element as arguments. The function can return any data type, if it is not
        /// a string, it's ToString() method will be used to convert it to a string.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/text/#text2
        /// </url>

        public CQ Text(Func<int,string,object> func) {

            int count=0;
            foreach (IDomElement obj in Elements)
            {
                string oldText = Text(obj);
                string newText = func(count, oldText).ToString();
                if (oldText != newText)
                {
                    SetChildText(obj, newText);
                }
                count++;
            }
            return this;
        }

        /// <summary>
        /// Add elements to the set of matched elements from a selector or an HTML fragment.
        /// </summary>
        ///
        /// <param name="selector">
        /// A CSS selector.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/add/
        /// </url>

        public CQ Add(string selector)
        {
            return Add(Select(selector));
        }

        /// <summary>
        /// Add an element to the set of matched elements.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to add.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/add/
        /// </url>

        public CQ Add(IDomObject element)
        {
            return Add(Objects.Enumerate(element));
        }

        /// <summary>
        /// Add elements to the set of matched elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to add.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/add/
        /// </url>

        public CQ Add(IEnumerable<IDomObject> elements)
        {
            CQ res = new CQ(this);
            res.AddSelection(elements);
            return res;
        }

        /// <summary>
        /// Add elements to the set of matched elements from a selector or an HTML fragment.
        /// </summary>
        ///
        /// <param name="selector">
        /// A string representing a selector expression to find additional elements to add to the set of
        /// matched elements.
        /// </param>
        /// <param name="context">
        /// The point in the document at which the selector should begin matching; similar to the context
        /// argument of the $(selector, context) method.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/add/
        /// </url>

        public CQ Add(string selector, IEnumerable<IDomObject> context)
        {
            return Add(Select(selector, context));
        }

        /// <summary>
        /// Add elements to the set of matched elements from a selector or an HTML fragment.
        /// </summary>
        ///
        /// <param name="selector">
        /// A string representing a selector expression to find additional elements to add to the set of
        /// matched elements.
        /// </param>
        /// <param name="context">
        /// The point in the document at which the selector should begin matching; similar to the context
        /// argument of the $(selector, context) method.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/add/
        /// </url>

        public CQ Add(string selector,IDomObject context)
        {
            return Add(Select(selector,context));
        }

        /// <summary>
        /// Adds the specified class, or each class in a space-separated list, to each of the set of
        /// matched elements.
        /// </summary>
        ///
        /// <param name="className">
        /// One or more class names to be added to the class attribute of each matched element.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/addclass/
        /// </url>

        public CQ AddClass(string className)
        {
            foreach (var item in Elements)
            {
                item.AddClass(className);
            }
            return this;
        }

        /// <summary>
        /// Add or remove one or more classes from each element in the set of matched elements, depending
        /// on either the class's presence.
        /// </summary>
        ///
        /// <param name="classes">
        /// One or more class names (separated by spaces) to be toggled for each element in the matched
        /// set.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/toggleClass/
        /// </url>

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
        /// Add or remove one or more classes from each element in the set of matched elements, depending
        /// on the value of the switch argument.
        /// </summary>
        ///
        /// <param name="classes">
        /// One or more class names (separated by spaces) to be toggled for each element in the matched
        /// set.
        /// </param>
        /// <param name="addRemoveSwitch">
        /// a boolean value that determine whether the class should be added (true) or removed (false).
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/toggleClass/
        /// </url>

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

        //public CQ ToggleClass(bool addRemoveSwitch)
        //{

        //}

        /// <summary>
        /// Determine whether any of the matched elements are assigned the given class.
        /// </summary>
        ///
        /// <param name="className">
        /// The class name to search for.
        /// </param>
        ///
        /// <returns>
        /// true if the class exists on any of the elements, false if not.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/hasclass/
        /// </url>

        public bool HasClass(string className)
        {
            
            IDomElement el = FirstElement();

            return el==null ? false :
                el.HasClass(className);
        }

        /// <summary>
        /// Insert content, specified by the parameter, to the end of each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="content">
        /// One or more HTML strings to append.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/append/
        /// </url>

        public CQ Append(params string[] content)
        {
            return Append(mergeContent(content));
        }

        /// <summary>
        /// Insert the element, specified by the parameter, to the end of each element in the set of
        /// matched elements.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to exclude.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/append/
        /// </url>

        public CQ Append(IDomObject element)
        {
            return Append(Objects.Enumerate(element));
        }

        /// <summary>
        /// Insert the sequence of elements, specified by the parameter, to the end of each element in
        /// the set of matched elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to be excluded.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/append/
        /// </url>

        public CQ Append(IEnumerable<IDomObject> elements)
        {
            CQ ignoredOutput;
            return Append(elements, out ignoredOutput);
        }

        /// <summary>
        /// Appends a func.
        /// </summary>
        ///
        /// <param name="func">
        /// A delegate to a function that returns an HTML string to insert at the end
        /// of each element in the set of matched elements. Receives the index position of the element in
        /// the set and the old HTML value of the element as arguments. Within the function, this refers
        /// to the current element in the set.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/append/
        /// </url>

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

        /// <summary>
        /// Insert content, specified by the parameter, to the end of each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="func">
        /// A delegate to a function that returns an IDomElement to insert at the end of each element in
        /// the set of matched elements. Receives the index position of the element in the set and the
        /// old HTML value of the element as arguments. Within the function, this refers to the current
        /// element in the set.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/append/
        /// </url>

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

        /// <summary>
        /// Insert content, specified by the parameter, to the end of each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="func">
        /// A delegate to a function that returns a sequence of IDomElement objects to insert at the end
        /// of each element in the set of matched elements. Receives the index position of the element in
        /// the set and the old HTML value of the element as arguments. Within the function, this refers
        /// to the current element in the set.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/append/
        /// </url>

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
        /// Insert every element in the set of matched elements to the end of each element in the targets.
        /// </summary>
        ///
        /// <remarks>
        /// The .Append() and .appendTo() methods perform the same task. The major difference is in the
        /// syntax-specifically, in the placement of the content and target. With .Append(), the selector
        /// expression preceding the method is the container into which the content is inserted. With
        /// .AppendTo(), on the other hand, the content precedes the method, either as a selector
        /// expression or as markup created on the fly, and it is inserted into the target container.
        /// </remarks>
        ///
        /// <param name="target">
        /// A selector that results in HTML to which the selection set will be appended.
        /// </param>
        ///
        /// <returns>
        ///  A CQ object containing all the elements added
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/appendTo/
        /// </url>

        public CQ AppendTo(params string[] target)
        {
            CQ output;
            new CQ(mergeSelections(target)).Append(SelectionSet,out output);

            return output;

        }

        /// <summary>
        /// Insert every element in the set of matched elements to the end of the target.
        /// </summary>
        ///
        /// <param name="target">
        /// The element to which the elements in the current selection set should be appended.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object containing the target elements.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/appendTo/
        /// </url>

        public CQ AppendTo(IDomObject target)
        {
            return AppendTo(Objects.Enumerate(target));
        }

        /// <summary>
        /// Insert every element in the set of matched elements to the end of the target.
        /// </summary>
        ///
        /// <param name="targets">
        /// The targets to which the current selection will be appended.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object containing the target elements.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/appendTo/
        /// </url>

        public CQ AppendTo(IEnumerable<IDomObject> targets)
        {
            CQ output;
            EnsureCsQuery(targets).Append(SelectionSet, out output);
            return output;
        }

        /// <summary>
        /// Insert content, specified by the parameter, to the beginning of each element in the set of
        /// matched elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// One or more elements.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object representing the inserte content.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/prepend/
        /// </url>

        public CQ Prepend(params IDomObject[] elements)
        {
            return Prepend(Objects.Enumerate(elements));
        }

        /// <summary>
        /// Insert content, specified by the parameter, to the beginning of each element in the set of
        /// matched elements.
        /// </summary>
        ///
        /// <param name="selector">
        /// One or more selectors or HTML strings.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/prepend/
        /// </url>

        public CQ Prepend(params string[] selector)
        {
            return Prepend(mergeContent(selector));
        }

        /// <summary>
        /// Insert content, specified by the parameter, to the beginning of each element in the set of
        /// matched elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to be inserted.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/prepend/
        /// </url>

        public CQ Prepend(IEnumerable<IDomObject> elements)
        {
            CQ ignoredOutput;
            return Prepend(elements, out ignoredOutput);
        }

        /// <summary>
        /// Insert content, specified by the parameter, to the beginning of each element in the set of
        /// matched elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to be inserted.
        /// </param>
        /// <param name="insertedElements">
        /// A CQ object containing all the elements added.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/prepend/
        /// </url>

        public CQ Prepend(IEnumerable<IDomObject> elements, out CQ insertedElements)
        {
            insertedElements = New();
            bool first = true;
            
            foreach (var target in Elements)
            {
                /// <summary>
                /// For the first iteration, the elements can be moved. For successive iterations, a clone
                /// must be insterted.
                /// </summary>

                IEnumerable<IDomObject> content =
                    first ? 
                        elements : 
                        EnsureCsQuery(onlyElements(elements)).Clone().SelectionSet;


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

        /// <summary>
        /// Insert every element in the set of matched elements to the beginning of the target.
        /// </summary>
        ///
        /// <param name="target">
        /// One or more HTML strings that will be targeted.
        /// </param>
        ///
        /// <returns>
        /// A CQ object containing all the elements added
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/prependTo/
        /// </url>

        public CQ PrependTo(params string[] target)
        {

            CQ output;
            new CQ(mergeSelections(target)).Prepend(SelectionSet, out output);

            return output;
        }

        /// <summary>
        /// Insert every element in the set of matched elements to the beginning of the target.
        /// </summary>
        ///
        /// <param name="targets">
        /// The targets to which the current selection will be appended.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object representing the target elements.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/prependTo/
        /// </url>

        public CQ PrependTo(IEnumerable<IDomObject> targets)
        {
            CQ output;
            EnsureCsQuery(targets).Prepend(SelectionSet, out output);
            return output;
        }

        /// <summary>
        /// Get the value of an attribute for the first element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the attribute to get.
        /// </param>
        ///
        /// <returns>
        /// A string of the attribute value.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/attr/#attr1
        /// </url>

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
                            if (HtmlData.IsBoolean(name))
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
                            (el.NodeName =="INPUT" || el.NodeName=="SELECT" || el.NodeName=="OPTION")) {
                            return Val();
                        } else if (name=="value" && el.NodeName =="TEXTAREA") {
                            return el.InnerText;
                        }
                        break;
                }
                
            }
            return null;
        }

        /// <summary>
        /// Get the value of an attribute for the first element in the set of matched elements.
        /// </summary>
        ///
        /// <remarks>
        /// This is a CsQuery extension. Attribute values are always stored as strings internally, in
        /// line with their being created and represented as HTML string data. This method simplifies
        /// converting to another type such as integer for attributes that represent strongly-type values.
        /// </remarks>
        ///
        /// <typeparam name="T">
        /// Type to which the attribute value should be converted.
        /// </typeparam>
        /// <param name="name">
        /// The name of the attribute to get.
        /// </param>
        ///
        /// <returns>
        /// A strongly-typed value representing the attribute.
        /// </returns>

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
        ///
        /// <exception cref="InvalidOperationException">
        /// Thrown when attemting to change the type of an INPUT element that already exists on the DOM.
        /// </exception>
        ///
        /// <param name="name">
        /// THe attribute name.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        public CQ Attr(string name, IConvertible value)
        {

            // Make sure attempts to pass a JSON string end up a the right place
            if (Objects.IsJson(name) && value.GetType()==typeof(bool))
            {
                return AttrSet(name, (bool)value);
            }
            
            // jQuery 1.7 compatibility
            bool isBoolean = HtmlData.IsBoolean(name);
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
                if ((e.NodeName == "INPUT" || e.NodeName == "BUTTON") && name == "type"
                    && !e.IsDisconnected)
                {
                    throw new InvalidOperationException("Can't change type of \"input\" elements that have already been added to a DOM");
                }
                e.SetAttribute(name, val);
            }
            return this;
        }

        /// <summary>
        /// Map an object to a set of attributes name/values and set those attributes on each object in
        /// the selection set.
        /// </summary>
        ///
        /// <remarks>
        /// The jQuery API uses the same method "Attr" for a wide variety of purposes. For Attr and Css
        /// methods, the overloads that we would like to use to match all the ways the method is used in
        /// the jQuery API don't work out in the strongly-typed world of C#. To resolved this, the
        /// methods AttrSet and CssSet were created for methods where an object or a string of JSON are
        /// passed (a map) to set multiple methods.
        /// </remarks>
        ///
        /// <param name="map">
        /// An object whose properties names represent attribute names, or a string that is valid JSON
        /// data that represents an object of attribute names/values.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/attr/#attr2
        /// </url>

        public CQ AttrSet(object map)
        {
            return AttrSet(map, false);
        }

        /// <summary>
        /// Map an object to attributes, optionally using "quickSet" to set other properties in addition
        /// to the attributes.
        /// </summary>
        ///
        /// <param name="map">
        /// An object whose properties names represent attribute names, or a string that is valid JSON
        /// data that represents an object of attribute names/values.
        /// </param>
        /// <param name="quickSet">
        /// If true, set any css from a sub-map object passed with "css", html from "html", inner text
        /// from "text", and css from "width" and "height" properties.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        public CQ AttrSet(object map, bool quickSet=false)
        {
            IDictionary<string, object> dict;

            string cssString = map as string;
            if (cssString != null && Objects.IsJson((cssString)))
            {
                dict = ParseJSON<IDictionary<string, object>>(cssString);
            }
            else
            {
                dict = Objects.ToExpando(map);
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
        /// Remove an attribute from each element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="name">
        /// The attribute name to remove.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/removeAttr/
        /// </url>

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
        /// Remove a property from the set of matched elements.
        /// </summary>
        ///
        /// <remarks>
        /// In CsQuery, there is no distinction between an attribute and a property. In a real browser
        /// DOM, this method will actually remove a property from an element, causing consequences such
        /// as the inability to set it later. In CsQuery, the DOM is stateless and is simply a
        /// representation of the HTML that created it. This method is included for compatibility, but
        /// causes no special behavior.
        /// </remarks>
        ///
        /// <param name="name">
        /// The property (attribute) name to remove.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/removeProp/
        /// </url>

        public CQ RemoveProp(string name)
        {
            return RemoveAttr(name);
        }

        /// <summary>
        /// Insert content, specified by the parameter, before each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="selector">
        /// A CSS selector that determines the elements to insert.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/before/
        /// </url>

        public CQ Before(string selector)
        {
            return Before(Select(selector));
        }

        /// <summary>
        /// Insert the element, specified by the parameter, before each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to insert.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/before/
        /// </url>

        public CQ Before(IDomObject element)
        {
            return Before(Objects.Enumerate(element));
        }

        /// <summary>
        /// Insert each element, specified by the parameter, before each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to insert.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/before/
        /// </url>

        public CQ Before(IEnumerable<IDomObject> elements)
        {
            EnsureCsQuery(elements).InsertAtOffset(SelectionSet, 0);
            return this;
        }

        /// <summary>
        /// Insert content, specified by the parameter, after each element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="selector">
        /// A CSS selector that determines the elements to insert.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/after/
        /// </url>

        public CQ After(string selector)
        {
            return After(Select(selector));
        }

        /// <summary>
        /// Insert an element, specified by the parameter, after each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to insert.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/after/
        /// </url>

        public CQ After(IDomObject element)
        {
            return After(Objects.Enumerate(element));
        }

        /// <summary>
        /// Insert elements, specified by the parameter, after each element in the set of matched
        /// elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to insert.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/after/
        /// </url>

        public CQ After(IEnumerable<IDomObject> elements)
        {
            EnsureCsQuery(elements).InsertAtOffset(SelectionSet, 1);
            return this;
        }

        /// <summary>
        /// Remove the parents of the set of matched elements from the DOM, leaving the matched elements
        /// in their place.
        /// </summary>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/unwrap/
        /// </url>

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

        /// <summary>
        /// Wrap an HTML structure around each element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="wrappingSelector">
        /// A string that is either a selector or a string of HTML that defines the structure to wrap
        /// around the set of matched elements.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/wrap/
        /// </url>

        public CQ Wrap(string wrappingSelector)
        {
            return Wrap(Select(wrappingSelector));
        }

        /// <summary>
        /// Wrap an HTML structure around each element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="element">
        /// An element which is the structure to wrap around the selection set.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/wrap/
        /// </url>

        public CQ Wrap(IDomObject element)
        {
            return Wrap(Objects.Enumerate(element));
        }

        /// <summary>
        /// Wrap an HTML structure around each element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="wrapper">
        /// A sequence of elements that is the structure to wrap around the selection set. There may be
        /// multiple elements but there should be only one innermost element in the sequence.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/wrap/
        /// </url>

        public CQ Wrap(IEnumerable<IDomObject> wrapper)
        {
            return Wrap(wrapper, false);
        }

        /// <summary>
        /// Wrap an HTML structure around all elements in the set of matched elements.
        /// </summary>
        ///
        /// <param name="wrappingSelector">
        /// A string that is either a selector or a string of HTML that defines the structure to wrap
        /// around the set of matched elements.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/wrapall/
        /// </url>

        public CQ WrapAll(string wrappingSelector)
        {
            return WrapAll(Select(wrappingSelector));
        }

        /// <summary>
        /// Wrap an HTML structure around all elements in the set of matched elements.
        /// </summary>
        ///
        /// <param name="element">
        /// An element which is the structure to wrap around the selection set.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/wrapall/
        /// </url>

        public CQ WrapAll(IDomObject element)
        {
            return WrapAll(Objects.Enumerate(element));
        }

        /// <summary>
        /// Wrap an HTML structure around all elements in the set of matched elements.
        /// </summary>
        ///
        /// <param name="wrapper">
        /// A sequence of elements that is the structure to wrap around each element in the selection
        /// set. There may be multiple elements but there should be only one innermost element in the
        /// sequence.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/wrapall/
        /// </url>

        public CQ WrapAll(IEnumerable<IDomObject> wrapper)
        {
            return Wrap(wrapper, true);
        }

        private CQ Wrap(IEnumerable<IDomObject> wrapper, bool keepSiblingsTogether)
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
        ///
        /// <param name="selector">
        /// An HTML snippet or elector expression specifying the structure to wrap around the content of
        /// the matched elements.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/wrapinner/
        /// </url>

        public CQ WrapInner(string selector)
        {
            return WrapInner(Select(selector));
        }

        /// <summary>
        /// Wrap an HTML structure around the content of each element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="wrapper">
        /// A sequence of elements that is the structure to wrap around the content of the selection set.
        /// There may be multiple elements but there should be only one innermost element in the sequence.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/wrapinner/
        /// </url>

        public CQ WrapInner(IDomObject wrapper)
        {
            return WrapInner(Objects.Enumerate(wrapper));
        }

        /// <summary>
        /// Wrap an HTML structure around the content of each element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="wrapper">
        /// A sequence of elements that is the structure to wrap around the content of the selection set.
        /// There may be multiple elements but there should be only one innermost element in the sequence.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/wrapinner/
        /// </url>

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

        /// <summary>
        /// Get the children of each element in the set of matched elements, optionally filtered by a
        /// selector.
        /// </summary>
        ///
        /// <param name="filter">
        /// A selector that must match each element returned.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/children/
        /// </url>

        public CQ Children(string filter=null)
        {
            return filterIfSelector(filter, SelectionChildren());
        }

        /// <summary>
        /// Description: Get the siblings of each element in the set of matched elements, optionally
        /// filtered by a selector.
        /// </summary>
        ///
        /// <param name="selector">
        /// A selector used to filter the siblings.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/siblings/
        /// </url>

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
        ///
        /// <returns>
        /// A new CQ object that contains a clone of each element in the original selection set.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/clone/
        /// </url>

        public CQ Clone()
        {
            CQ csq = new CQ();
            
            
            // TODO: The type of document needs to be implemented as a factory. THere are certainly other places
            // where this choice should be made.
            
            if (Document is IDomFragment)
            {
                csq.CreateNewFragment();
            }
            else
            {
                csq.CreateNewDocument();
            }

            foreach (IDomObject elm in SelectionSet)
            {
                IDomObject clone = elm.Clone();
                csq.Document.ChildNodes.AddAlways(clone);
                csq.AddSelection(clone);
            }
            return csq;
        }

        /// <summary>
        /// Get the first ancestor element that matches the selector, beginning at the current element
        /// and progressing up through the DOM tree.
        /// </summary>
        ///
        /// <param name="selector">
        /// A CSS selector.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/closest/#closest1
        /// </url>

        public CQ Closest(string selector)
        {
            CQ matchTo = Select(selector);
            return Closest(matchTo);
        }

        /// <summary>
        /// Return the element passed by parameter, if it is an ancestor of any elements in the selection
        /// set.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to target.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/closest/#closest1
        /// </url>

        public CQ Closest(IDomObject element)
        {
            return Closest(Objects.Enumerate(element));
        }

        /// <summary>
        /// Get the first ancestor element of any element in the seleciton set that is also one of the
        /// elements in the sequence passed by parameter, beginning at the current element and
        /// progressing up through the DOM tree.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to target.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/closest/#closest1
        /// </url>

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
        /// Get the children of each element in the set of matched elements, including text and comment
        /// nodes.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/contents/
        /// </url>

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
        /// Set one or more CSS properties for the set of matched elements from JSON data.
        /// </summary>
        ///
        /// <param name="map">
        /// An object whose properties names represent css property names, or a string that is valid JSON
        /// data that represents an object of css style names/values.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/css/#css2
        /// </url>

        public CQ CssSet(object map)
        {
            IDictionary<string, object> dict;
            if (Objects.IsJson(map))
            {
                dict = ParseJSON<IDictionary<string, object>>((string)map);
            }
            else
            {
                dict = Objects.ToExpando(map);
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
        /// Set one or more CSS properties for the set of matched elements.
        /// </summary>
        ///
        /// <remarks>
        /// By default, this method will validate that the CSS style name and value are valid CSS3. To
        /// assing a style without validatoin, use the overload of this method and set the "strict"
        /// parameter to false.
        /// </remarks>
        ///
        /// <param name="name">
        /// The name of the style.
        /// </param>
        /// <param name="value">
        /// The value of the style.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/css/#css2
        /// </url>

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
        /// Get the value of a style property for the first element in the set of matched elements, and
        /// converts to a numeric type T. Any numeric type strings are ignored when converting to numeric
        /// values.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The type. This should probably be a numeric type, but the method will attempt to convert to
        /// any IConvertible type passed.
        /// </typeparam>
        /// <param name="style">
        /// The name of the CSS style to retrieve.
        /// </param>
        ///
        /// <returns>
        /// A value of type T.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/css/#css1
        /// </url>

        public T Css<T>(String style) where T: IConvertible 
        {
            IDomElement el =FirstElement();
            if (el==null) {
                return default(T);
            }

            
            if (Objects.IsNumericType(typeof(T)))
            {IStringScanner scanner = Scanner.Create(el.Style[style] ?? "");
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
            else
            {
                return (T)Objects.ChangeType(el.Style[style] ?? "", typeof(T));
            }
        }

        /// <summary>
        /// Get the value of a style property for the first element in the set of matched elements.
        /// </summary>
        ///
        /// <param name="style">
        /// The name of the CSS style.
        /// </param>
        ///
        /// <returns>
        /// A string of the value of the named CSS style.
        /// </returns>

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
        /// Returns all values at named data store for the first element in the jQuery collection, as set
        /// by data(name, value). Put another way, this method constructs an object based on the names
        /// and values of any attributes starting with "data-".
        /// </summary>
        ///

        /// <summary>
        /// Gets the data.
        /// </summary>
        ///
        /// <returns>
        /// A dynamic object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/data/#data2
        /// </url>

        public IDynamicMetaObjectProvider Data()
        {
            var dataObj = new JsObject();
            IDictionary<string, object> data = dataObj;
            IDomElement obj = FirstElement();
            if (obj != null)
            {

                foreach (var item in obj.Attributes)
                {
                    if (item.Key.StartsWith("data-"))
                    {
                        data[item.Key.Substring(5)] = CQ.ParseJSON(item.Value);
                    }
                }
                return dataObj;
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// Store arbitrary data associated with the specified element, and render it as JSON on the
        /// element in a format that can be read by the jQuery "Data()" methods.
        /// </summary>
        ///
        /// <param name="key">
        /// The name of the key to associate with this data object.
        /// </param>
        /// <param name="data">
        /// An string to be associated with the key.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/data/#data1
        /// </url>

        public CQ Data(string key,string data)
        {
            foreach (IDomElement e in Elements)
            {
                e.SetAttribute("data-" + key, JSON.ToJSON(data));
            }
            return this;
        }

        /// <summary>
        /// Store arbitrary data associated with the specified element, and render it as JSON on the
        /// element in a format that can be read by the jQuery "Data()" methods.
        /// </summary>
        ///
        /// <remarks>
        /// Though the jQuery "Data" methods are designed to read the HTML5 "data-" attributes like the
        /// CsQuery version, jQuery Data keeps its data in an internal data store that is unrelated to
        /// the element attributes. This is not particularly necessary when working in C# since you have
        /// many other framwork options for managing data. Rather, this method has been implemented to
        /// simplify passing data back and forth between the client and server. You should be able to use
        /// CsQuery's Data methods to set arbitrary objects as data, and read them directly from the
        /// client using the jQuery data method. Bear and mind that because CsQuery intends to write
        /// every object you assign using "Data" as a JSON string on a "data-" attribute, there's a lot
        /// of conversion going on which will probably have imperfect results if you just try to use it
        /// as a way to attach an object to an element. It's therefore advised that you think of it as a
        /// way to get data to the client primarily.
        /// </remarks>
        ///
        /// <param name="key">
        /// The name of the key to associate with this data object.
        /// </param>
        /// <param name="data">
        /// An string containing properties to be mapped to JSON data.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/data/#data1
        /// </url>

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
        /// Convert an object to JSON and stores each named property as a data element.
        /// </summary>
        ///
        /// <remarks>
        /// Because of conflicts with the overloaded signatures compared to the jQuery API, the general
        /// Data method that maps an entire object has been implemented as DataSet.
        /// 
        /// Though the jQuery "Data" methods are designed to read the HTML5 "data-" attributes like the
        /// CsQuery version, jQuery Data keeps its data in an internal data store that is unrelated to
        /// the element attributes. This is not particularly necessary when working in C# since you have
        /// many other framwork options for managing data. Rather, this method has been implemented to
        /// simplify passing data back and forth between the client and server. You should be able to use
        /// CsQuery's Data methods to set arbitrary objects as data, and read them directly from the
        /// client using the jQuery data method. Bear and mind that because CsQuery intends to write
        /// every object you assign using "Data" as a JSON string on a "data-" attribute, there's a lot
        /// of conversion going on which will probably have imperfect results if you just try to use it
        /// as a way to attach an object to an element. It's therefore advised that you think of it as a
        /// way to get data to the client primarily.
        /// </remarks>
        ///
        /// <param name="data">
        /// An object containing properties which will be mapped to data attributes.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/data/#data1
        /// </url>

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
        /// Returns an object or value at named data store for the first element in the jQuery collection,
        /// as set by data(name, value).
        /// </summary>
        ///
        /// <param name="key">
        /// The named key to identify the data, resulting in access to an attribute named "data-{key}".
        /// </param>
        ///
        /// <returns>
        /// An object representing the stored data. This could be a value type, or a POCO with properties
        /// each containing other objects or values, depending on the data that was initially set.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/data/#data2
        /// </url>

        public object Data(string key)
        {
            string data = First().Attr("data-" + key);
            
            return JSON.ParseJSON(data);
        }

        /// <summary>
        /// Returns an object or value at named data store for the first element in the jQuery collection,
        /// as set by data(name, value).
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The type to which to cast the data. This type should match the type used when setting the
        /// data initially, or be a type that is compatible with the JSON data structure stored in the
        /// data attribute.
        /// </typeparam>
        /// <param name="key">
        /// The name of the key to associate with this data object.
        /// </param>
        ///
        /// <returns>
        /// An object of type T.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/data/#data2
        /// </url>

        public T Data<T>(string key)
        {
            string data = First().Attr("data-" + key);
            return JSON.ParseJSON<T>(data);
        }

        /// <summary>
        /// Remove all data- attributes from the element.
        /// </summary>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/removeData/
        /// </url>

        public CQ RemoveData()
        {
            return RemoveData((string)null);
        }

        /// <summary>
        /// Remove a previously-stored piece of data identified by a key.
        /// </summary>
        ///
        /// <param name="key">
        /// A string naming the piece of data to delete, or pieces of data if the string has multiple
        /// values separated by spaces.
        /// </param>
        ///
        /// <returns>
        /// THe current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/removeData/
        /// </url>

        public CQ RemoveData(string key)
        {
            foreach (IDomElement el in Elements)
            {
                List<string> toRemove = new List<string>();
                foreach (var kvp in el.Attributes)
                {
                    bool match = String.IsNullOrEmpty(key) ?
                        kvp.Key.StartsWith("data-") :
                        kvp.Key == "data-" + key;
                    if (match)
                    {
                        toRemove.Add(kvp.Key);
                    }
                }
                foreach (string attr in toRemove)
                {
                    el.RemoveAttribute(attr);
                }
            }
            return this;
        }

        /// <summary>
        /// Remove all data from an element.
        /// </summary>
        ///
        /// <param name="keys">
        /// An array or space-separated string naming the pieces of data to delete.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/removeData/
        /// </url>

        public CQ RemoveData(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                RemoveData(key);

            }
            return this;
        }

        /// <summary>
        /// Returns data as a string, with no attempt to parse it from JSON. This is the equivalent of
        /// using the Attr("data-{key}") method.
        /// </summary>
        ///
        /// <param name="key">
        /// The key identifying the data.
        /// </param>
        ///
        /// <returns>
        /// A string.
        /// </returns>

        public string DataRaw(string key)
        {
            return First().Attr("data-" + key);
        }

        /// <summary>
        /// Iterate over each matched element, calling the delegate passed by parameter for each element.
        /// If the delegate returns false, the iteration is stopped.
        /// </summary>
        ///
        /// <remarks>
        /// The overloads of Each the inspect the return value have a different method name (EachUntil)
        /// because the C# compiler will not choose the best-matchine method when passing method groups.
        /// See: http://stackoverflow.com/questions/2057146/compiler-ambiguous-invocation-error-anonymous-
        /// method-and-method-group-with-fun.
        /// </remarks>
        ///
        /// <param name="func">
        /// A function delegate returning a boolean, and accepting an integer and an IDomObject
        /// parameter. The integer is the zero-based index of the current iteration, and the IDomObject
        /// is the current element.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/each/
        /// </url>

        public CQ EachUntil(Func<int, IDomObject, bool> func)
        {
            int index = 0;
            foreach (IDomObject obj in Selection)
            {
                if (!func(index++, obj))
                {
                    break;
                }

            }
            return this;
        }

        /// <summary>
        /// Iterate over each matched element, calling the delegate passed by parameter for each element.
        /// If the delegate returns false, the iteration is stopped.
        /// </summary>
        ///
        /// <remarks>
        /// The overloads of Each the inspect the return value have a different method name (EachUntil)
        /// because the C# compiler will not choose the best-matchine method when passing method groups.
        /// See: http://stackoverflow.com/questions/2057146/compiler-ambiguous-invocation-error-anonymous-
        /// method-and-method-group-with-fun.
        /// </remarks>
        ///
        /// <param name="func">
        /// A function delegate returning a boolean.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/each/
        /// </url>

        public CQ EachUntil(Func<IDomObject, bool> func)
        {

            foreach (IDomObject obj in Selection)
            {
                if (!func(obj))
                {
                    break;
                }
            }
            return this;
        }

        /// <summary>
        /// Iterate over each matched element, calling the delegate passed by parameter for each element
        /// </summary>
        ///
        /// <param name="func">
        /// A delegate accepting a single IDomObject paremeter
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/each/
        /// </url>

        public CQ Each(Action<IDomObject> func)
        {
            foreach (IDomObject obj in Selection)
            {
                func(obj);
            }
            return this;
        }

        /// <summary>
        /// Iterate over each matched element, calling the delegate passed by parameter for each element.
        /// </summary>
        ///
        /// <param name="func">
        /// A delegate accepting an integer parameter, and an IDomObject paremeter. The integer is the
        /// zero-based index of the current iteration.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/each/
        /// </url>

        public CQ Each(Action<int,IDomObject> func)
        {
            int index = 0;
            foreach (IDomObject obj in Selection)
            {
                func(index++,obj);
            }
            return this;
        }

        /// <summary>
        /// Reduce the set of matched elements to the one at the specified index.
        /// </summary>
        ///
        /// <param name="index">
        /// The zero-based index within the current selection set to match.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/eq/
        /// </url>

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
        /// Get the descendants of each element in the current set of matched elements, filtered by a
        /// selector.
        /// </summary>
        ///
        /// <param name="selector">
        /// A string containing a selector expression to match elements against.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/find/
        /// </url>

        public CQ Find(string selector)
        {
            return FindImpl(new Selector(selector));
        }

        /// <summary>
        /// Get the descendants of each element in the current set of matched elements, filtered by a
        /// sequence of elements or CQ object.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to match against.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/find/
        /// </url>

        public CQ Find(IEnumerable<IDomObject> elements)
        {
           return FindImpl(new Selector(elements));
        }

        /// <summary>
        /// Get a single element, if it is a descendant of the current selection set.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to matc.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/find/
        /// </url>

        public CQ Find(IDomObject element)
        {
            return FindImpl(new Selector(element));
        }

        private CQ FindImpl(Selector selector)
        {
            CQ csq = New();
            csq.AddSelection(selector.Select(Document, this));
            csq.Selectors = selector;
            return csq;
        }

        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's
        /// test.
        /// </summary>
        ///
        /// <param name="selector">
        /// A string containing a selector expression to match the current set of elements against.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/filter/
        /// </url>

        public CQ Filter(string selector)
        {
            return new CQ(filterElements(SelectionSet, selector));

        }

        /// <summary>
        /// Reduce the set of matched elements to those that matching the element passed by parameter.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to match.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/filter/
        /// </url>
        
        public CQ Filter(IDomObject element) {
            return Filter(Objects.Enumerate(element));
        }

        /// <summary>
        /// Reduce the set of matched elements to those matching any of the elements in a sequence passed
        /// by parameter.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to match.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/filter/
        /// </url>
        
        public CQ Filter(IEnumerable<IDomObject> elements) {
            CQ filtered = new CQ(this);
            filtered.SelectionSet.IntersectWith(elements);
            return filtered;            
        }

        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's
        /// test.
        /// </summary>
        ///
        /// <remarks>
        /// This method doesn't offer anything that can't easily be accomplished with a LINQ "where"
        /// query but is included for completeness.
        /// </remarks>
        ///
        /// <param name="function">
        /// A function used as a test for each element in the set.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/filter/
        /// </url>

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

        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's
        /// test.
        /// </summary>
        ///
        /// <remarks>
        /// This method doesn't offer anything that can't easily be accomplished with a LINQ "where"
        /// query but is included for completeness.
        /// </remarks>
        ///
        /// <param name="function">
        /// A function used as a test for each element in the set.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/filter/
        /// </url>

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
        /// Return a specific element from the selection set.
        /// </summary>
        ///
        /// <param name="index">
        /// The zero-based index of the element to be returned.
        /// </param>
        ///
        /// <returns>
        /// An IDomObject.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/get/.
        /// </url>

        public IDomObject this[int index]
        {
            get
            {
                return Get(index);
            }
        }

        /// <summary>
        /// Select elements and return a new CSQuery object.
        /// </summary>
        ///
        /// <remarks>
        /// The "Select" method is the default CsQuery method. It's overloads are identical to the
        /// overloads of the CQ object's property indexer (the square-bracket notation) and it functions
        /// the same way. This is analogous to the default jQuery method, e.g. $(...).
        /// </remarks>
        ///
        /// <param name="selector">
        /// A string containing a selector expression.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>

        public CQ Select(string selector)
        {
            CQ csq = New();
            csq.Selectors = new Selector(selector);

            // If the selector is HTML create it as a new fragment so it can be indexed & traversed upon
            // (This comment is a placeholder for implementing document fragments properly)
            // IDomDocument dom = selectors.IsHtml ? new DomFragment(selector.ToCharArray()) : Document;
            
            csq.AddSelection(csq.Selectors.Select(Document));
            return csq;
        }

        /// <summary>
        /// Select elements and return a new CSQuery object.
        /// </summary>
        ///
        /// <remarks>
        /// The "Select" method is the default CsQuery method. It's overloads are identical to the
        /// overloads of the CQ object's property indexer and it functions the same way. This is
        /// analogous to the default jQuery method, e.g. $(...).
        /// </remarks>
        ///
        /// <param name="selector">
        /// A string containing a selector expression.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>

        public CQ this[string selector]
        {
            get
            {
                return Select(selector);
            }
        }

        /// <summary>
        /// Return a new CQ object wrapping an element.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to wrap.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>

        public CQ Select(IDomObject element)
        {
            CQ csq = new CQ(element,this);
            return csq;
        }

        /// <summary>
        /// Return a new CQ object wrapping an element.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to wrap.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>

        public CQ this[IDomObject element]
        {
            get
            {
                return Select(element);
            }
        }

        /// <summary>
        /// Return a new CQ object wrapping a sequence of elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to wrap
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>
        
        public CQ Select(IEnumerable<IDomObject> elements)
        {
            CQ csq = new CQ(elements,this);
            return csq;
        }

        /// <summary>
        /// Return a new CQ object wrapping a sequence of elements.
        /// </summary>
        ///
        /// <param name="element">
        /// The elements to wrap.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>

        public CQ this[IEnumerable<IDomObject> element]
        {
            get
            {
                return Select(element);
            }
        }

        /// <summary>
        /// Select elements from within a context.
        /// </summary>
        ///
        /// <param name="selector">
        /// A string containing a selector expression.
        /// </param>
        /// <param name="context">
        /// The point in the document at which the selector should begin matching; similar to the context
        /// argument of the CQ.Create(selector, context) method.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>

        public CQ Select(string selector, IDomObject context)
        {
            var selectors = new Selector(selector);
            var selection = selectors.Select(Document, context);

            CQ csq = new CQ(selection, this);
            csq.Selectors = selectors;
            return csq;
        }

        /// <summary>
        /// Select elements from within a context.
        /// </summary>
        ///
        /// <param name="selector">
        /// A string containing a selector expression.
        /// </param>
        /// <param name="context">
        /// The point in the document at which the selector should begin matching; similar to the context
        /// argument of the CQ.Create(selector, context) method.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>

        public CQ this[string selector, IDomObject context]
        {
            get
            {
                return Select(selector, context);
            }
        }

        /// <summary>
        /// Select elements from within a context.
        /// </summary>
        ///
        /// <param name="selector">
        /// A string containing a selector expression.
        /// </param>
        /// <param name="context">
        /// The points in the document at which the selector should begin matching; similar to the
        /// context argument of the CQ.Create(selector, context) method. Only elements found below the
        /// members of the sequence in the document can be matched.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>

        public CQ Select(string selector, IEnumerable<IDomObject> context)
        {
            var selectors = new Selector(selector);

            IEnumerable<IDomObject> selection = selectors.Select(Document, context);

            CQ csq = new CQ(selection, (CQ)this);
            csq.Selectors = selectors;
            return csq;
        }

        /// <summary>
        /// Select elements from within a context.
        /// </summary>
        ///
        /// <param name="selector">
        /// A string containing a selector expression.
        /// </param>
        /// <param name="context">
        /// The points in the document at which the selector should begin matching; similar to the
        /// context argument of the CQ.Create(selector, context) method. Only elements found below the
        /// members of the sequence in the document can be matched.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/jQuery/#jQuery1
        /// </url>

        public CQ this[string selector, IEnumerable<IDomObject> context]
        {
            get
            {
                return Select(selector, context);
            }
        }

        /// <summary>
        /// Reduce the set of matched elements to the first in the set.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object containing the first element in the set, or no elements if the source was
        /// empty.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/first/
        /// </url>

        public CQ First()
        {
            return Eq(0);
        }

        /// <summary>
        /// Reduce the set of matched elements to the last in the set.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object containing the last element in the set, or no elements if the source was
        /// empty.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/last/
        /// </url>

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
        ///
        /// <remarks>
        /// The jQuery docs say "This is roughly equivalent to calling .css('display', 'none')." With
        /// CsQuery, it is exactly equivalent. Unlike jQuery, CsQuery does not store the current value of
        /// the "display" style and restore it, because there is no concept of "effective style" in
        /// CsQuery. We don't attempt to calculate the actual style that would be in effect since we
        /// don't do any style sheet parsing. Instead, this method really just sets display: none. When
        /// showing again, any "display" style is removed.
        /// 
        /// This means if you were to assign a non-default value for "display" such as "inline" to a div,
        /// then Hide(), then Show(), it would no longer be displayed inline, as it would in jQuery.
        /// Since CsQuery is not used interactively (yet, anyway), this sequence of events seems unlikely,
        /// and supporting it exactly as jQuery does seems unnecessary. This functionality could
        /// certainly be added in the future.
        /// </remarks>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/hide/
        /// </url>

        public CQ Hide()
        {
            foreach (IDomElement e in Elements)
            {
                e.Style["display"]= "none";
            }
            return this;

        }

        /// <summary>
        /// Display the matched elements.
        /// </summary>
        ///
        /// <remarks>
        /// This method simply removes the "display: none" css style, if present. See
        /// <see cref="T:CsQuery.CQ.Hide"/> for an explanation of how this differs from jQuery.
        /// </remarks>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/show/
        /// </url>

        public CQ Show()
        {
            foreach (IDomElement e in Elements)
            {
                if (e.Style["display"] == "none")
                {
                    e.RemoveStyle("display");
                }
            }
            return this;
        }

        /// <summary>
        /// Display or hide the matched elements.
        /// </summary>
        ///
        /// <returns>
        /// The curren CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/toggle/
        /// </url>

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
        /// Display or hide the matched elements based on the value of the parameter.
        /// </summary>
        ///
        /// <param name="isVisible">
        /// true to show the matched elements, or false to hide them.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/toggle/
        /// </url>

        public CQ Toggle(bool isVisible)
        {
            return isVisible ?
                Show() :
                Hide();
        }

        /// <summary>
        /// Search for a given element from among the matched elements.
        /// </summary>
        ///
        /// <returns>
        /// The index of the element, or -1 if it was not found.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/index/
        /// </url>

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
        /// Returns the position of the current selection within the new selection defined by "selector".
        /// </summary>
        ///
        /// <param name="selector">
        /// The selector string.
        /// </param>
        ///
        /// <returns>
        /// The zero-based index of the selection within the new selection
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/index/
        /// </url>

        public int Index(string selector)
        {
            var selection = Select(selector);
            return selection.Index(SelectionSet);
        }

        /// <summary>
        /// Returns the position of the element passed in within the selection set.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to exclude.
        /// </param>
        ///
        /// <returns>
        /// The zero-based index of "element" within the selection set, or -1 if it was not a member of
        /// the current selection.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/index/
        /// </url>

        public int Index(IDomObject element)
        {
            int index = -1;
            if (element != null)
            {
                int count = 0;
                foreach (IDomObject el in SelectionSet)
                {
                    if (ReferenceEquals(el, element))
                    {
                        index = count;
                        break;
                    }
                    count++;
                }
            }
            return index;
        }

        /// <summary>
        /// Returns the position of the first element in the sequence passed by parameter within the
        /// current selection set..
        /// </summary>
        ///
        /// <param name="elements">
        /// The element to look for.
        /// </param>
        ///
        /// <returns>
        /// The zero-based index of the first element in the sequence within the selection.
        /// </returns>

        public int Index(IEnumerable<IDomObject> elements)
        {
            return Index(elements.FirstOrDefault());
            
        }

        /// <summary>
        /// Insert every element in the set of matched elements after the target.
        /// </summary>
        ///
        /// <summary>
        /// Inserts an after described by target.
        /// </summary>
        ///
        /// <param name="target">
        /// The target to insert after.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/insertAfter/
        /// </url>

        public CQ InsertAfter(IDomObject target)
        {
            return InsertAtOffset(target,1);
        }

        /// <summary>
        /// Insert every element in the set of matched elements after each element in the target sequence.
        /// </summary>
        ///
        /// <remarks>
        /// If there is a single element in the target, the elements in the selection set will be moved
        /// before the target (not cloned). If there is more than one target element, however, cloned
        /// copies of the inserted element will be created for each target after the first, and that new
        /// set (the original element plus clones) is returned.
        /// </remarks>
        ///
        /// <param name="target">
        /// A sequence of elements or a CQ object.
        /// </param>
        ///
        /// <returns>
        /// The set of elements inserted, including the original elements and any clones made if there
        /// was more than one target.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/insertAfter/
        /// </url>

        public CQ InsertAfter(IEnumerable<IDomObject> target) {
            CQ output;
            InsertAtOffset(target, 1, out output);
            return output;
        }

        /// <summary>
        /// Insert every element in the set of matched elements after the target.
        /// </summary>
        ///
        /// <remarks>
        /// If there is a single element in the resulting set of the selection created by the parameter
        /// selector, then the original elements in this object's selection set will be moved before it.
        /// If there is more than one target element, however, cloned copies of the inserted element will
        /// be created for each target after the first, and that new set (the original element plus
        /// clones) is returned.
        /// </remarks>
        ///
        /// <param name="selectorTarget">
        /// A selector identifying the target elements after which each element in the current set will
        /// be inserted.
        /// </param>
        ///
        /// <returns>
        /// The set of elements inserted, including the original elements and any clones made if there
        /// was more than one target.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/insertAfter/
        /// </url>

        public CQ InsertAfter(string selectorTarget)
        {
            return InsertAfter(Select(selectorTarget));
        }

        /// <summary>
        /// Insert every element in the set of matched elements before each elemeent in the selection set
        /// created from the target selector.
        /// </summary>
        ///
        /// <remarks>
        /// If there is a single element in the resulting set of the selection created by the parameter
        /// selector, then the original elements in this object's selection set will be moved before it.
        /// If there is more than one target element, however, cloned copies of the inserted element will
        /// be created for each target after the first, and that new set (the original element plus
        /// clones) is returned.
        /// </remarks>
        ///
        /// <param name="selector">
        /// A selector. The matched set of elements will be inserted before the element(s) specified by
        /// this selector.
        /// </param>
        ///
        /// <returns>
        /// The set of elements inserted, including the original elements and any clones made if there
        /// was more than one target.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/insertBefore/
        /// </url>

        public CQ InsertBefore(string selector)
        {
            return InsertBefore(Select(selector));
        }

        /// <summary>
        /// Insert every element in the set of matched elements before the target.
        /// </summary>
        ///
        /// <param name="target">
        /// The element to which the elements in the current selection set should inserted after.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/insertBefore/
        /// </url>

        public CQ InsertBefore(IDomObject target)
        {
            return InsertAtOffset(target, 0);
        }

        /// <summary>
        /// Insert every element in the set of matched elements before the target.
        /// </summary>
        ///
        /// <remarks>
        /// If there is a single element in the target, the elements in the selection set will be moved
        /// before the target (not cloned). If there is more than one target element, however, cloned
        /// copies of the inserted element will be created for each target after the first, and that new
        /// set (the original element plus clones) is returned.
        /// </remarks>
        ///
        /// <param name="target">
        /// A sequence of elements or a CQ object that is the target; each element in the selection set
        /// will be inserted after each element in the target.
        /// </param>
        ///
        /// <returns>
        /// The set of elements inserted, including the original elements and any clones made if there
        /// was more than one target.
        /// </returns>
        ///
        /// <url>
        /// http://api.jquery.com/insertBefore/
        /// </url>

        public CQ InsertBefore(IEnumerable<IDomObject> target)
        {
            CQ output;
            InsertAtOffset(target, 0, out output);
            return output;
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

       
        /// <summary>
        /// Reduce the set of matched elements to a subset beginning with the 0-based index provided
        /// </summary>
        /// <param name="start">The 0-based index at which to begin selecting</param>
        /// <returns></returns>
        public CQ Slice(int start)
        {
            return Slice(start, SelectionSet.Count);
        }
        /// <summary>
        /// Reduce the set of matched elements to a subset specified by a range of indices. 
        /// </summary>
        /// <param name="start">The 0-based index at which to begin selecting</param>
        /// <param name="end">The 0-based index at which to stop selecting (up to but not including this index)</param>
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
                end = SelectionSet.Count;
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
            
            CQ output = New();
            HashSet<IDomElement> targets = new HashSet<IDomElement>();
            if (selector != null)
            {
                targets.AddRange(Select(selector).Elements);
            }
            var filtered = filterElementsIgnoreNull(parentsImpl(Elements, targets), filter);
            output.SelectionSet.Order = SelectionSetOrder.Descending;
            output.SelectionSet.AddRange(filtered);
            
            return output;
        }
       
        protected IEnumerable<IDomElement> parentsImpl(IEnumerable<IDomElement> source, HashSet<IDomElement> until)
        {

            HashSet<IDomElement> alreadyAdded = new HashSet<IDomElement>();

            foreach (var item in source)
            {
                int depth = item.Depth;
                IDomElement parent = item.ParentNode as IDomElement;
                while (parent != null && !until.Contains(parent))
                {
                    if (alreadyAdded.Add(parent))
                    {
                        yield return parent;
                    }
                    else
                    {
                        break;
                    }

                    parent = parent.ParentNode as IDomElement;
                }
            }


            //return results.Select(item => item.Item3);
            //var comp = new parentComparer();
            //return results.OrderBy(item=>item,comp).Select(item => item.Item3);
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

            if (HtmlData.IsBoolean(name))
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
            if (Length > 0 && HtmlData.IsBoolean(name))
            {
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

        public CQ RemoveClass()
        {
            Elements.ForEach(item =>
            {
                item.ClassName = "";
            });
            return this;
        }
        /// <summary>
        /// Remove a single class, multiple classes, or all classes from each element in the set of matched elements.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public CQ RemoveClass(string className)
        {
            
            foreach (IDomElement e in Elements)
            {
                if (!String.IsNullOrEmpty(className))
                {
                    e.RemoveClass(className);
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
                switch(e.NodeNameID) {
                    case HtmlData.tagTEXTAREA:
                        return e.InnerText;
                    case HtmlData.tagINPUT:
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
                    case HtmlData.tagSELECT:
                        string result = String.Empty;
                        // TODO optgroup handling (just like the setter code)
                        var options =Find("option");
                        if (options.Length==0) {
                            return null;
                        }
                        
                        foreach (IDomElement child in options)
                        {
                            bool disabled = child.HasAttribute("disabled") 
                                || (child.ParentNode.NodeName == "OPTGROUP" && child.ParentNode.HasAttribute("disabled"));

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
                    case HtmlData.tagOPTION:
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
                    case "TEXTAREA":
                        // should we delete existing children first? they should not exist
                        e.InnerText = val;
                        break;
                    case "INPUT":
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
                    case "SELECT":
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
        /// Set the CSS width of each element in the set of matched elements.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CQ Width(int value)
        {
            return Width(value.ToString() + "px");
        }

        /// <summary>
        /// Set the CSS width of each element in the set of matched elements.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Set the CSS height of each element in the set of matched elements.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check the current matched set of elements against a selector and return true if at least one of these elements matches the selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public bool Is(IEnumerable<IDomObject> elements)
        {
            HashSet<IDomObject> els = new HashSet<IDomObject>(elements);
            els.IntersectWith(SelectionSet);
            return els.Count > 0;
        }

        /// <summary>
        /// Check the current matched set of elements against a selector and return true if at least one of these elements matches the selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public bool Is(IDomObject element)
        {
            return SelectionSet.Contains(element);
        }

        #endregion

        #region private helpers for public methods

        private CQ nextPrevImpl(string selector, bool next)
        {
            return filterIfSelector(selector,
                ForEach(Elements, (input) =>
                {
                    return next ? input.NextElementSibling : input.PreviousElementSibling;
                }), next ? SelectionSetOrder.Ascending : SelectionSetOrder.Descending);
        }
        private CQ nextPrevAllImpl(string filter, bool next)
        {
            return filterIfSelector(filter, ForEachMany(Elements, (input) =>
            {
                return nextPrevAllImpl(input, next);
            }), next ? SelectionSetOrder.Ascending : SelectionSetOrder.Descending);
        }

        private CQ nextPrevUntilImpl(string selector, string filter, bool next)
        {
            if (string.IsNullOrEmpty(selector))
            {
                return next ? NextAll(filter) : PrevAll(filter);
            }

            HashSet<IDomElement> untilEls = new HashSet<IDomElement>(Select(selector).Elements);
            return filterIfSelector(filter, ForEachMany(Elements, (input) =>
            {
                return nextPrevUntilFilterImpl(input, untilEls, next);
            }), next ? SelectionSetOrder.Ascending : SelectionSetOrder.Descending);
        }

        private IEnumerable<IDomObject> nextPrevAllImpl(IDomObject input, bool next)
        {
            IDomObject item = next ? input.NextElementSibling : input.PreviousElementSibling;
            while (item != null)
            {
                yield return item;
                item = next ? item.NextElementSibling : item.PreviousElementSibling;
            }
        }

        private IEnumerable<IDomObject> nextPrevUntilFilterImpl(IDomObject input, HashSet<IDomElement> untilEls, bool next)
        {
            foreach (IDomElement el in nextPrevAllImpl(input, next))
            {
                if (untilEls.Contains(el))
                {
                    break;
                }
                yield return el;
            }
        }

        /// <summary>
        /// Helper for public Text() function to act recursively.
        /// </summary>
        ///
        /// <param name="sb">
        /// .
        /// </param>
        /// <param name="elements">
        /// .
        /// </param>

        private void Text(StringBuilder sb, IEnumerable<IDomObject> elements)
        {
            IDomObject lastElement = null;
            foreach (IDomObject obj in elements)
            {
                string text = Text(obj);
                
                if (lastElement != null && obj.Index > 0
                   && obj.PreviousSibling != lastElement 
                    && text.Trim()!="")
                {
                    sb.Append(" ");
                }
                sb.Append(Text(obj));
                lastElement = obj;
                
            }
        }

        /// <summary>
        /// Get the combined text contents of this and all child elements
        /// </summary>
        ///
        /// <param name="obj">
        /// The object.
        /// </param>
        ///
        /// <returns>
        /// A string containing the text contents of the selection.
        /// </returns>
        private string Text(IDomObject obj)
        {
            switch (obj.NodeType)
            {
                case NodeType.TEXT_NODE:
                case NodeType.CDATA_SECTION_NODE:
                case NodeType.COMMENT_NODE:
                    return obj.NodeValue;
                    
                case NodeType.ELEMENT_NODE:
                case NodeType.DOCUMENT_FRAGMENT_NODE:
                case NodeType.DOCUMENT_NODE:
                    StringBuilder sb = new StringBuilder();
                    Text(sb, obj.ChildNodes);
                    return sb.ToString();
                case NodeType.DOCUMENT_TYPE_NODE:
                    return "";
                default: 
                    return "";
            }
        }

        /// <summary>
        /// Sets a child text for this element, using the text node type appropriate for this element's type
        /// </summary>
        ///
        /// <param name="el">
        /// The element to add text to
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>

        private void SetChildText(IDomElement el, string text)
        {
            if (el.ChildrenAllowed)
            {
                el.ChildNodes.Clear();

                // Element types that cannot have HTML contents should not have the value encoded.
                // use DomInnerText node for those node types to preserve the raw text value

                IDomText textEl = el.InnerHtmlAllowed ?
                    new DomText(text) :
                    new DomInnerText(text);

                el.ChildNodes.Add(textEl);
            }

        }

        /// <summary>
        /// Support for InsertAfter and InsertBefore. An offset of 0 will insert before the current element. 1 after.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private CQ InsertAtOffset(IDomObject target, int offset)
        {
            int index = target.Index;

            foreach (var item in SelectionSet)
            {
                target.ParentNode.ChildNodes.Insert(index + offset, item);
                index++;
            }
            return this;
        }

        /// <summary>
        /// Append each element passed by parameter to each element in the selection set. The inserted
        /// elements are returned.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to be excluded.
        /// </param>
        /// <param name="insertedElements">
        /// A CQ object containing all the elements added.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        private CQ Append(IEnumerable<IDomObject> elements, out CQ insertedElements)
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
        #endregion


    }


    
}
