using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.StringScanner
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
        HtmlTagSelectorStart=4096,
        HtmlTagSelectorExceptStart=8192,
        HtmlTagEnd=16384,
        HtmlTagAny=32768,
        HtmlTagNameStart = 65536,
        HtmlTagNameExceptStart = 131072,
        HtmlIDNameExceptStart = 262144,
        HTMLIDNameSelectorExceptStart=524288

    }
}
