using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;
using CsQuery.Implementation;
using CsQuery.HtmlParser;
using CsQuery.StringScanner;


namespace CsQuery
{
    public partial class CQ
    {


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
            CQ csq;
            var sel = new Selector(selector);

            if (sel.IsHmtl)
            {
                csq = CQ.CreateFragment(ExpandSelfClosingTags(selector));
                // when creating a fragment as a selector, the selection set is a living document
                // REMOVED - causes other problems.
                //csq.SetSelection(csq.Document.ChildNodes);
                csq.CsQueryParent = this;
            }
            else
            {
                // When running a true "Select" (which runs against the DOM, versus methods that operate
                // against the selection set) we should use the CsQueryParent document, which is the DOM
                // that sourced this.

                var selectorSource = CsQueryParent == null ?
                    Document :
                    CsQueryParent.Document;

                csq = New();
                csq.Selector = sel;
                csq.SetSelection(csq.Selector.Select(selectorSource),
                        SelectionSetOrder.Ascending);
                
            }
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
            CQ csq = NewInstance(element, this);
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
            CQ csq = NewInstance(elements, this);
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

            CQ csq = NewInstance(selection, this);
            csq.Selector = selectors;
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

            CQ csq = NewInstance(selection, (CQ)this);
            csq.Selector = selectors;
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
        /// Parses an HTML snippet so self-closing HTML tags are expanded (except those that cannot have children)
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        ///
        /// <returns>
        /// The string with the items expanded
        /// </returns>

        protected string ExpandSelfClosingTags(string html)
        {
            return SelfClosingTag.Replace(html, match=>{
                var keyword = match.Groups[1].Value;
                if (HtmlData.ChildrenAllowed(keyword))
                {
                    return "<" + keyword + "></" + keyword + ">";
                }
                else
                {
                    return match.Value;
                }
            });
        }

        private string GetKeywordOnly(string matchString)
        {
            int pos = matchString.IndexOfAny(SelfClosingTagStoppers);
            if (pos > 0)
            {
                return matchString.Substring(0, pos);
            }
            else
            {
                return matchString;
            }
            
        }

        private static readonly char[] SelfClosingTagStoppers = (CharacterData.charsHtmlSpaceArray.Concat('/')).ToArray();

        private static readonly Regex SelfClosingTag = new Regex(@"<([a-zA-z]\w*)\s?/>");
    }
}
