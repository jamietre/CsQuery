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
        /// Indicates that unexpected HTML closing tags should be removed from the DOM. The alternative is to leave them, which will
        /// generally cause browsers to display them as text.
        /// </summary>
        
        RemoveMismatchedCloseTags = 1,
        
        /// <summary>
        /// Remove comments from the output
        /// </summary>
        
        RemoveComments = 2,
        
        /// <summary>
        /// Add quotes around each attribute value, whether or not they are needed. The alternative is to only 
        /// use quotes when they are necesssary to delimit the value (e.g. because it includes spaces or other quote characters)
        /// </summary>
        
        QuoteAllAttributes = 4
        //,ValidateCss = 8
    }
 
}
