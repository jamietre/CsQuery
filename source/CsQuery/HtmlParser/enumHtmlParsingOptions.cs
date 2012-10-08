using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    /// <summary>
    /// The options used when parsing strings of HTML
    /// </summary>

    [Flags]
    public enum HtmlParsingOptions : byte
    {
        /// <summary>
        /// No options applied.
        /// </summary>
        None=0,
       /// <summary>
       /// Tags may be self-closing.
       /// </summary>

        AllowselfClosingTags=1,


        /// <summary>
        /// Comments are ignored entirely.
        /// </summary>
        
        IgnoreComments=2
    }
}
