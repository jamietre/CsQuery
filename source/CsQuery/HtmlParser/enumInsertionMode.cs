using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser
{

    public enum InsertionMode : byte
    {
        Default = 0,
        Script = 1,
        Text = 2,
        Invalid = 3
    }
}
