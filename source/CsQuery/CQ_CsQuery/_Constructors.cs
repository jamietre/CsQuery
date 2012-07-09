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
        /// Create a new CQ object from an HTML character array.
        /// </summary>
        ///
        /// <param name="html">
        /// The html of the new document
        /// </param>


        public CQ(char[] html)
        {
            CreateNewFragment(html, HtmlParsingMode.Content);
        }

        /// <summary>
        /// Create a new CQ object wrapping a single element
        /// </summary>
        ///
        /// <param name="element">
        /// The element
        /// </param>

        public CQ(IDomObject element)
        {
            Document = element.Document;
            AddSelection(element);
        }


        /// <summary>
        /// Create a new CsQuery from a single DOM element.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to wrap.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>

        public CQ(IDomObject element, CQ context)
        {
            CsQueryParent = context;
            SetSelection(element,SelectionSetOrder.OrderAdded);
        }
        
        /// <summary>
        /// Create a new CQ object from an HTML character array 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public CQ(string html)
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
        /// Create a new CsQuery object from HTML, and assign CSS from a JSON string, within a context
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
        public CQ(string selector, string cssJson, CQ context)
        {
            _CQ(selector, context);
            AttrSet(cssJson);
        }
        /// <summary>
        /// Create a new CsQuery object from HTML, and assign CSS, within a context
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
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
            //bool first = true;
            //if (elements is CQ)
            //{
            //    CsQueryParent = (CQ)elements;
            //    first = false;
            //}

            //foreach (IDomObject el in elements)
            //{
            //    if (first)
            //    {
            //        Document = el.Document;
            //        first = false;
            //    }
            //    SelectionSet.Add(el);
            //}
            var list = elements.ToList();

            if (elements is CQ)
            {
                CQ asCq = (CQ)elements;
                CsQueryParent = asCq;
                Document = asCq.Document;
            }
            else
            {
                // not actually a CQ object, we can get the Document the els are bound to from one of the
                // elements. 

                var el = elements.FirstOrDefault();
                if (el != null)
                {
                    Document = el.Document;
                }
            }
            SetSelection(list, SelectionSetOrder.OrderAdded);
        }

        /// <summary>
        /// Create a new CsQuery object from a set of DOM elements, using the DOM from context.
        /// </summary>
        ///
        /// <param name="elements">
        /// .
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>

        public CQ(IEnumerable<IDomObject> elements, CQ context)
        {
            CsQueryParent = context;
            AddSelection(elements);
        }

        #endregion

        #region implicit constructors

        ///// <summary>
        ///// Create a new CQ object from html
        ///// </summary>
        ///// <param name="text"></param>
        ///// <returns></returns>
        //public static implicit operator CQ(string html)
        //{
        //    return CQ.Create(html);
        //}

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

        protected void CreateNewDocument()
        {
            Document = new DomDocument();
            FinishCreatingNewDocument();
        }
        protected void CreateNewFragment()
        {
            Document = new DomFragment();
            FinishCreatingNewDocument();
        }

        protected void CreateNewFragment(IEnumerable<IDomObject> elements)
        {
            Document = new DomFragment(elements);
            AddSelection(Document.ChildNodes);
            FinishCreatingNewDocument();
        }
        /// <summary> 
        /// Replace the existing DOM with the html (or empty if no parameter passed)
        /// </summary>
        /// <param name="html"></param>
        protected void CreateNewDocument(char[] html, HtmlParsingMode htmlParsingMode)
        {
            Document = new DomDocument(html,htmlParsingMode);
            HtmlElementFactory.ReorganizeStrandedTextNodes(Document);
            AddSelection(Document.ChildNodes);
            FinishCreatingNewDocument();
        }
        /// <summary>
        /// Replace the existing DOM with the html (or empty if no parameter passed)
        /// </summary>
        /// <param name="html"></param>
        protected void CreateNewFragment(char[] html, HtmlParsingMode htmlParsingMode)
        {
            Document = new DomFragment(html,htmlParsingMode);
            SetSelection(Document.ChildNodes,SelectionSetOrder.Ascending);
            FinishCreatingNewDocument();
        }
        private void FinishCreatingNewDocument()
        {
            Document.DomRenderingOptions = CQ.DefaultDomRenderingOptions;
        }
        #endregion
    }
}
