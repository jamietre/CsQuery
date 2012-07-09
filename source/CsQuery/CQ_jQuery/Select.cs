using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;
using CsQuery.Implementation;

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

            CQ csq = New();
            csq.Selector = new Selector(selector);
            csq.SetSelection(csq.Selector.Select(Document),
                csq.Selector.IsHmtl ?
                    SelectionSetOrder.OrderAdded :
                    SelectionSetOrder.Ascending);
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
            CQ csq = new CQ(element, this);
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
            CQ csq = new CQ(elements, this);
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

            CQ csq = new CQ(selection, (CQ)this);
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

    }
}
