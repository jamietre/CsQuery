using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Implementation;
using CsQuery.Engine;
using CsQuery.HtmlParser;

namespace CsQuery
{
    public partial class CQ
    {
        #region regular constructors

        /// <summary>
        /// Creates a new, empty CQ object.
        /// </summary>

        public CQ()
        {

        }

        /// <summary>
        /// Create a new CQ object from an HTML character array. Synonymous with
        /// <see cref="CQ.Create(char[])"/>
        /// </summary>
        ///
        /// <param name="html">
        /// The html of the new document.
        /// </param>

        public CQ(char[] html)
        {
            CreateNewFragment(html, HtmlParsingMode.Content);
        }

        /// <summary>
        /// Create a new CQ object wrapping a single element.
        /// </summary>
        /// 
        /// <remarks>
        /// This differs from the <see cref="CQ.Create(IDomObject)"/> method in that this document is still
        /// related to its owning document; this is the same as if the element had just been selected.
        /// The Create method, conversely, creates an entirely new Document context contining a single
        /// element (a clone of this element).
        /// </remarks>
        ///
        /// <param name="element">
        /// The element.
        /// </param>

        public CQ(IDomObject element)
        {
            Document = element.Document;
            AddSelection(element);
        }

        /// <summary>
        /// Create a new CQ object wrapping a single DOM element, in the context of another CQ object.
        /// </summary>
        ///
        /// <remarks>
        /// This differs from the overload accepting a single IDomObject parameter in that it associates
        /// the new object with a previous object, as if it were part of a selector chain. In practice
        /// this will rarely make a difference, but some methods such as <see cref="CQ.End"/> use
        /// this information.
        /// </remarks>
        ///
        /// <param name="element">
        /// The element to wrap.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>

        public CQ(IDomObject element, CQ context)
        {
            ConfigureNewInstance(this, element, context);
        }

        private CQ NewInstance(IDomObject element, CQ context)
        {
            var cq = NewInstance();
            ConfigureNewInstance(cq, element, context);
            return cq;
        }
        private void ConfigureNewInstance(CQ dom, IDomObject element, CQ context)
        {
            dom.CsQueryParent = context;
            dom.SetSelection(element, SelectionSetOrder.OrderAdded);
        }
        
        /// <summary>
        /// Create a new CQ object from an HTML character array 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public CQ(string html)
        {
            ConfigureNewInstance(this, html);
        }
        private CQ NewInstance(string html)
        {
            var cq = NewInstance();
            ConfigureNewInstance(cq, html);
            return cq;
        }
        private void ConfigureNewInstance(CQ dom, string html)
        {
            CreateNewFragment(Support.StringToCharArray(html), HtmlParsingMode.Content);
        }
        /// <summary>
        /// Create a new CsQuery object using an existing instance and a selector. if the selector is null or missing, then
        /// it will contain no selection results.
        /// </summary>
        /// <param name="selector">A valid CSS selector</param>
        /// <param name="context">The context</param>
        public CQ(string selector, CQ context)
        {
            _CQ(selector,context);
        }

        private void _CQ(string selector, CQ context) {
             CsQueryParent = context;

            if (!String.IsNullOrEmpty(selector))
            {
                Selector = new Selector(selector);

                SetSelection(Selector.Select(Document, context),
                    Selector.IsHmtl ?
                        SelectionSetOrder.OrderAdded :
                        SelectionSetOrder.Ascending);
            }
            
        }

        /// <summary>
        /// Create a new CsQuery object from a selector HTML, and assign CSS from a JSON string, within a context.
        /// </summary>
        ///
        /// <param name="selector">
        /// The 
        /// </param>
        /// <param name="cssJson">
        /// The JSON containing CSS
        /// </param>
        /// <param name="context">
        /// The context
        /// </param>

        public CQ(string selector, string cssJson, CQ context)
        {
            _CQ(selector, context);
            AttrSet(cssJson);
        }

        /// <summary>
        /// Create a new CsQuery object from a selector or HTML, and assign CSS, within a context.
        /// </summary>
        ///
        /// <param name="selector">
        /// The selector or HTML markup
        /// </param>
        /// <param name="css">
        /// The object whose property names and values map to CSS
        /// </param>
        /// <param name="context">
        /// The context
        /// </param>

        public CQ(string selector, object css, CQ context)
        {
            _CQ(selector, context);
            AttrSet(css);
        }
        


        /// <summary>
        /// Create a new CsQuery object from an existing CsQuery object (or any set of DOM elements).
        /// If the source is a unassociated list of DOM elements, the context of the first element will become
        /// the context of the new CsQuery object.
        /// </summary>
        /// <param name="elements"></param>
        public CQ(IEnumerable<IDomObject> elements)
        {
            ConfigureNewInstance(this,elements);
        }

        private CQ NewInstance(IEnumerable<IDomObject> elements)
        {
            var cq = NewInstance();
            ConfigureNewInstance(cq,elements);
            return cq;
        }
        private void ConfigureNewInstance(CQ dom,IEnumerable<IDomObject> elements)
        {
            var list = elements.ToList();

            if (elements is CQ)
            {
                CQ asCq = (CQ)elements;
                dom.CsQueryParent = asCq;
                dom.Document = asCq.Document;
            }
            else
            {
                // not actually a CQ object, we can get the Document the els are bound to from one of the
                // elements. 

                var el = elements.FirstOrDefault();
                if (el != null)
                {
                    dom.Document = el.Document;
                }
            }
            dom.SetSelection(list, SelectionSetOrder.OrderAdded);
        }
        /// <summary>
        /// Create a new CsQuery object from a set of DOM elements, assigning the 2nd parameter as a context for this object.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements that make up the selection set in the new object
        /// </param>
        /// <param name="context">
        /// A CQ object that will be assigned as the context for this one.
        /// </param>

        public CQ(IEnumerable<IDomObject> elements, CQ context)
        {
            ConfigureNewInstance(this,elements, context);
        }
        private CQ NewInstance(IEnumerable<IDomObject> elements, CQ context)
        {
            var cq = NewInstance();
            ConfigureNewInstance(cq,elements, context);
            return cq;
        }

        /// <summary>
        /// Configures a new instance for a sequence of elements and an existing context.
        /// </summary>
        ///
        /// <param name="elements">
        /// A sequence of elements.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>

        private void ConfigureNewInstance(CQ dom,IEnumerable<IDomObject> elements, CQ context)
        {
            dom.CsQueryParent = context;
            dom.AddSelection(elements);
        }

        #endregion

        #region implicit constructors

        /// <summary>
        /// Create a new CQ object from html.
        /// </summary>
        ///
        /// <param name="html">
        /// A string of HTML
        /// </param>


        public static implicit operator CQ(string html)
        {
            return CQ.Create(html);
        }

       
        ///// <summary>
        ///// Create a new CQ object from html
        ///// </summary>
        ///// <param name="text"></param>
        ///// <returns></returns>
        //public static implicit operator CQ(char[] html)
        //{
        //    return CQ.Create(html);
        //}

        ///// <summary>
        ///// Create a new CQ object from an element
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static implicit operator CQ(DomObject obj)
        //{
        //    return CQ.Create(obj);
        //}

        #endregion

        #region Internal DOM creation methods

        /// <summary>
        /// Bind this instance to a new empty DomDocument configured with the default options.
        /// </summary>

        protected void CreateNewDocument()
        {
            Document = new DomDocument();
            FinishCreatingNewDocument();
        }

        /// <summary>
        /// Bind this instance to a new empty DomFragment configured with the default options.
        /// </summary>

        protected void CreateNewFragment()
        {
            Document = new DomFragment();
            FinishCreatingNewDocument();
        }

        /// <summary>
        /// Bind this instance to a new DomFragment created from a sequence of elements.
        /// </summary>
        ///
        /// <param name="elements">
        /// The elements to provide the source for this object's DOM.
        /// </param>

        protected void CreateNewFragment(IEnumerable<IDomObject> elements)
        {

            Document = DomDocument.Create(elements.Clone(), HtmlParsingMode.Fragment);
            AddSelection(Document.ChildNodes);
            FinishCreatingNewDocument();
        }

        /// <summary>
        /// Bind this instance to a new DomDocument created from HTML using the specified parsing mode.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        /// <param name="htmlParsingMode">
        /// The HTML parsing mode.
        /// </param>

        protected void CreateNewDocument(char[] html, HtmlParsingMode htmlParsingMode)
        {
            //Document = new DomDocument(html,htmlParsingMode);
            Document = DomDocument.Create(html.AsString(),HtmlParsingMode.Document);
            HtmlParser.Obsolete.HtmlElementFactory.ReorganizeStrandedTextNodes(Document);
            AddSelection(Document.ChildNodes);
            FinishCreatingNewDocument();
        }

        /// <summary>
        /// Bind this instance to a new DomFragment created from HTML using the specified parsing mode.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        /// <param name="htmlParsingMode">
        /// The HTML parsing mode.
        /// </param>

        protected void CreateNewFragment(char[] html, HtmlParsingMode htmlParsingMode)
        {
            //Document = new DomFragment(html,htmlParsingMode);
            Document = DomDocument.Create(html.AsString(),htmlParsingMode);
             
            //  enumerate ChildNodes when creating a new fragment to be sure the selection set only
            //  reflects the original document. 
            
            SetSelection(Document.ChildNodes.ToList(),SelectionSetOrder.Ascending);
            FinishCreatingNewDocument();
        }

        /// <summary>
        /// Apply the default rendering options to the new document.
        /// </summary>

        private void FinishCreatingNewDocument()
        {
            Document.DomRenderingOptions = CsQuery.Config.DomRenderingOptions;
        }
        
        #endregion
    }
}
