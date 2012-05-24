using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
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
        /// <param name="selector"></param>
        /// <param name="context"></param>
        public CQ(string selector, CQ context)
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
        protected void Create(string selector, CQ context)
        {
            // when creating a new CsQuery from another, leave Dom blank - it will be populated automatically with the
            // contents of the selector.
            CsQueryParent = context;

            if (!String.IsNullOrEmpty(selector))
            {
                Selectors = new SelectorChain(selector);
                AddSelectionRange(Selectors.Select(Document, context.Children()));
            }
        }
        /// <summary>
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
        /// Create a new CsQuery object from a set of DOM elements, using the DOM of the first element.
        /// could contain more than one context)
        /// </summary>
        /// <param name="elements"></param>
        public CQ(IEnumerable<IDomObject> elements, CQ context)
        {
            CsQueryParent = context;
            AddSelectionRange(elements);
        }

        /// <summary>
        /// Creates a new DOM. This will DESTROY any existing DOM. This is not the same as Select.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public void Load(string html)
        {
            Load((html ?? "").ToCharArray());
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

        #region private methods

        protected void Load(char[] html)
        {
            Clear();

            CreateNewDom(html);
            HtmlParser.DomElementFactory factory = new HtmlParser.DomElementFactory(Document);
            if (html != null)
            {
                foreach (IDomObject obj in factory.CreateObjects())
                {
                    Document.ChildNodes.Add(obj);
                    AddSelectionRange(Document.ChildNodes);
                }

            }
        }
        /// <summary>
        /// Creates a new DOM. This will DESTROY any existing DOM. This is not the same as Select.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected void Load(IEnumerable<IDomObject> elements)
        {
            Clear();
            CreateNewDom();
            ClearSelections();
            Document.ChildNodes.AddRange(elements);
            AddSelectionRange(Document.ChildNodes);
        }
        protected void CreateNewDom(char[] html=null)
        {
            if (html!=null)
            {
                Document = new DomDocument(html);
            }
            else
            {
                Document = new DomDocument();
            }
            Document.DomRenderingOptions = CQ.DefaultDomRenderingOptions;
        }
        #endregion
    }
}
