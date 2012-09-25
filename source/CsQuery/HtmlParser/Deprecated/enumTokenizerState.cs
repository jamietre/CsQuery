using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser.Deprecated
{
    /// <summary>
    /// Values that represent the current state of the tokenizer for an IterationData object
    /// </summary>

    public enum TokenizerState : byte
    {
        /// <summary>
        /// The normal (default) state; means content / looking for tags.
        /// </summary>
        
        Default = 0,         
        
        /// <summary>
        /// The tokenizer is inside an opening tag and parsing attributes.
        /// </summary>
        
        TagStart = 1,     
        
        /// <summary>
        /// The tokenizer is finished parsing this node.
        /// </summary>
       
        Finished = 2
    }
}
