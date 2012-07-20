using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    /// <summary>
    /// Values that represent the HTML document type.
    /// </summary>

    public enum DocType: byte
    {
        /// <summary>
        /// HTML5
        /// </summary>
        HTML5 = 1,
        /// <summary>
        /// HTML4
        /// </summary>
        HTML4 = 2,
        /// <summary>
        /// XHTML -- all tags will be explicitly closed.
        /// </summary>
        XHTML = 3,
        /// <summary>
        /// An unsupported document type.
        /// </summary>
        Unknown = 4
    }
}
