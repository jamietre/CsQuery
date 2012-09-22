using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser
{

    public enum HtmlParsingMode : byte
    {
        
        Auto = 0,
        Fragment = 1,
        Content = 2,
        Document = 3,
        
    }
}
