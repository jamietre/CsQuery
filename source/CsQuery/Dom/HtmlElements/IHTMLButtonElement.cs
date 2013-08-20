using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{   
    /// <summary>
    /// An HTML BUTTON element.
    /// </summary>
    ///
    /// <url>
    /// http://dev.w3.org/html5/markup/button.html
    /// </url>

    public interface IHTMLButtonElement : IDomElement
    {
        /// <summary>
        /// The form with which to associate the element.
        /// </summary>

        IHTMLFormElement Form {get;}
    }
}
