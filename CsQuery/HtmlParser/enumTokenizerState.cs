using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser
{
    public enum TokenizerState : byte
    {
        Default = 0,         // default parsing
        TagStart = 1,      // We're inside a tag opener
        Finished = 2
    }
}
