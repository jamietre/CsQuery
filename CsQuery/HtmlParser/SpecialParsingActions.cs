using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CsQuery.HtmlParser
{
    [Flags]
    public enum SpecialParsingActions
    {
        CloseParent = 1,
        CreateParent=2
    }
}
