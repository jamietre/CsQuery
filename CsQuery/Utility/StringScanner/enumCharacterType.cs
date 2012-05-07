using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.StringScanner
{
    [Flags]
    public enum CharacterType
    {
        Whitespace=1,
        Alpha=2,
        Number=4,
        NumberPart=8,
        Lower=16,
        Upper=32,
        Operator=64,
        Enclosing=128,
        Quote=256,
        Escape=512,
        Separator=1024,
        AlphaISO10646 = 2048,
        HtmlTagNameStart=4096,
        HtmlTagNameExceptStart=8192,
        HtmlTagEnd=16384,
        HtmlTagAny=32768
    }
}
