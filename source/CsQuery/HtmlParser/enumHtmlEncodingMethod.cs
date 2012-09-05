using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser
{
    public enum HtmlEncodingMethod
    {
        /// <summary>
        /// Complete encoding for all non-ascii characters
        /// </summary>
        HtmlEncodingFull = 1,
        /// <summary>
        /// Only carets and ampersand as required to create valid HTML.
        /// </summary>
        HtmlEncodingMinimum=2,
        /// <summary>
        /// No encoding.
        /// </summary>
        HtmlEncodingNone =3
    }
}
