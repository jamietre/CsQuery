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
            csq.Selector = Selector;
            return csq;
        }

    }
}
