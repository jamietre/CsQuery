using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Implementation;
using CsQuery.Engine;

namespace CsQuery
{
    public partial class CQ
    {
        #region regular constructors

        /// <summary>
        /// Creates a new, empty jQuery object.
        /// </summary>
        public CQ()
        {

        }

        /// <summary>
        /// Create a new CsQuery object using an existing instance and a selector. if the selector is null or missing, then
        /// it will contain no selection results.
        /// </summary>
        /// <param name="selector">A valid CSS selector</param>
        /// <param name="context">The context</param>
        public CQ(string selector, CQ context)
        {
            Create(selector, context);
        }

        /// <summary>
        /// Create a new CsQuery object using an existing instance and a selector. if the selector is null or missing, then
        /// it will contain no selection results.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
        public CQ(string selector, IDomElement context)
        {
            Create(selector, context);
        }

        /// <summary>
        /// Create a new CsQuery object from HTML, and assign CSS from a JSON string, within a context
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
        public CQ(string selector, string cssJson, CQ context)
        {
            Create(selector, context);
            AttrSet(cssJson);
        }
        /// <summary>
        /// Create a new CsQuery object from HTML, and assign CSS, within a context
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
        public CQ(string selector, object css, CQ context)
        {
            Create(selector, context);
            AttrSet(css);
        }
        
        /// Create a new CsQuery from a single elemement, using the element's context
        /// </summary>
        /// <param name="element"></param>
        public CQ(IDomObject element)
        {
            Document = element.Document;
            AddSelection(element);
        }
        /// <summary>
        /// Create a new CsQuery from a single DOM element
        /// </summary>
        /// <param name="element"></param>
        public CQ(IDomObject element, CQ context)
        {
            CsQueryParent = context;
            AddSelection(element);
        }
        /// <summary>
        /// Create a new CsQuery object from an existing CsQuery object (or any set of DOM elements).
        /// If the source is a unassociated list of DOM elements, the context of the first element will become
        /// the context of the new CsQuery object.
        /// </summary>
        /// <param name="elements"></param>
        public CQ(IEnumerable<IDomObject> elements)
        {
            //List<IDomObject> elList = new List<IDomObject>(elements);
            
            bool first = true;
            if (elements is CQ)
            {
                CsQueryParent = (CQ)elements;
                first = false;
            }
 
            foreach (IDomObject el in elements)
            {
                if (first)
                {
                    Document = el.Document;
                    first = false;
                }
                SelectionSet.Add(el);
            }
            
        }

        /// <summary>
        /// Create a new CsQuery object from a set of DOM elements, using the DOM from context
        /// </summary>
        /// <param name="elements"></param>
        public CQ(IEnumerable<IDomObject> elements, CQ context)
        {
            CsQueryParent = context;
            AddSelection(elements);
        }

        #endregion

        #region implicit constructors

        /// <summary>
        /// Create a new CQ object from html
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static implicit operator CQ(string html)
        {
            return CQ.Create(html);
        }

        /// <summary>
        /// Create a new CQ object from an element
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static implicit operator CQ(DomObject obj)
        {
            return CQ.Create(obj);
        }

        #endregion

        #region Internal DOM creation methods

        /// <summary>
        /// Load from an HTML document. This will try to parse it into a valid document using HTML5 rules. The key word is "try" this
        /// is not completely implemented.
        /// </summary>
        /// <param name="html"></param>
        protected void LoadDocument(char[] html)
        {
            Clear();
            //CreateNewDocument(html);
            
            //if (html != null)
            //{
            //    HtmlParser.HtmlElementFactory factory = new HtmlParser.HtmlElementFactory(Document);
            //    factory.ParseToDocument();
            //    AddSelectionRange(Document.ChildNodes);

               
            //}
            CreateNewDocument(html);
            ClearSelections();
            HtmlParser.HtmlElementFactory factory = new HtmlParser.HtmlElementFactory(Document);
            factory.ParseToDocument();
            AddSelection(Document.ChildNodes);

        }

        /// <summary>
        /// Load as if content - tag generation (EXCEPT for html/body) is enabled
        /// </summary>
        /// <param name="html"></param>
        protected void LoadContent(char[] html)
        {
            CreateNewFragment(html);
            ClearSelections();
            HtmlParser.HtmlElementFactory factory = new HtmlParser.HtmlElementFactory(Document);
            factory.GenerateOptionalElements = true;
            factory.IsDocument = false;
            Document.AddChildrenAlways(factory.Parse());
            AddSelection(Document.ChildNodes);

        }

        /// <summary>
        /// Load as if a fragment - no tag generation whatsoever
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected void LoadFragment(IEnumerable<IDomObject> elements)
        {
            Clear();
            CreateNewDocument();
            ClearSelections();
            Document.AddChildrenAlways(elements);
            AddSelection(Document.ChildNodes);
        }

        /// <summary>
        /// Creates a new fragment, e.g. HTML and BODY are not generated
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected void LoadFragment(char[] html)
        {
            Clear();
            CreateNewFragment(html);
            HtmlParser.HtmlElementFactory factory = new HtmlParser.HtmlElementFactory(Document);
            Document.AddChildrenAlways(factory.ParseAsFragment());
            AddSelection(Document.ChildNodes);

        }
        /// <summary>
        /// Replace the existing DOM with the html (or empty if no parameter passed)
        /// </summary>
        /// <param name="html"></param>
        protected void CreateNewDocument(char[] html=null)
        {
            Document = new DomDocument(html);
            FinishCreatingNewDocument();
        }
        /// <summary>
        /// Replace the existing DOM with the html (or empty if no parameter passed)
        /// </summary>
        /// <param name="html"></param>
        protected void CreateNewFragment(char[] html = null)
        {
            Document = new DomFragment(html);
            FinishCreatingNewDocument();
        }
        private void FinishCreatingNewDocument()
        {
            Document.DomRenderingOptions = CQ.DefaultDomRenderingOptions;
        }
        #endregion
    }
}
