using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser
{
    /// <summary>
    /// The Element Parsing scope 
    /// </summary>
    /// <url>http://dev.w3.org/html5/spec/parsing.html#stack-of-open-elements</url>

    public enum ElementScope : byte
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
