using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{   
    /// <summary>
    /// A regular DOM element
    /// </summary>
    public interface IDomElement : IDomContainer, IDomIndexedNode
    {

        /// <summary>
        /// The element is a block element
        /// </summary>
        /// <returntype>bool</returntype>
        bool IsBlock { get; }

        /// <summary>
        /// Returns the HTML for this element, but ignoring children/innerHTML
        /// </summary>
        /// <returns>A string of HTML</returns>
        /// <returntype>string</returntype>
        string ElementHtml();

        /// <summary>
        /// Get this element's index only among other elements (e.g. excluding text & other non-element node types)
        /// </summary>
        /// <returntype>int</returntype>
        int ElementIndex { get; }


    }
}
