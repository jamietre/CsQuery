using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    /// <summary>
    /// Flags specifying how the document should be rendered
    /// </summary>

    [Flags]
    public enum DomRenderingOptions
    {
        /// <summary>
        /// This option only appies to the old HTML parser. It is obsolete, has no effect, and will be
        /// removed in a future version of CsQuery.
        /// </summary>
        
        [Obsolete]
        RemoveMismatchedCloseTags = 1,
        
        /// <summary>
        /// Remove comments from the output
        /// </summary>
        
        RemoveComments = 2,
        
        /// <summary>
        /// Add quotes around each attribute value, whether or not they are needed. The alternative is to only 
        /// use quotes when they are necesssary to delimit the value (e.g. because it includes spaces or other quote characters)
        /// </summary>
        
        QuoteAllAttributes = 4,


        /// <summary>
        /// When true, text will be minimally HTML encoded (e.g. carets and ampersands). 
        /// </summary>
        
        HtmlEncodingMinimum = 8,

        /// <summary>
        /// When true, no HTML encoding of text nodes will be performed. This will supercede HtmlEncodingMinimum,
        /// of both are present, and may create invalid HTML since carets will not be encoded.
        /// </summary>
        
        HtmlEncodingNone=16
        
        //,ValidateCss = 8
    }
 
}
