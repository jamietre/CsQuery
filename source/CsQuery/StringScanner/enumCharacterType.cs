using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.StringScanner
{
    [Flags]
    public enum CharacterType
    {
        /// <summary>
        /// Whitespace
        /// </summary>

        Whitespace=0x01,
        /// <summary>
        /// Alpha charactersonly
        /// </summary>
        Alpha=0x02,
        /// <summary>
        /// Numeric characters only
        /// </summary>
        Number=0x04,
        /// <summary>
        /// Numbers plus non-numeric characters that can be part of a number
        /// </summary>
        NumberPart=0x08,
        
        /// <summary>
        /// Lowercase only
        /// </summary>
        Lower=0x10,
        /// <summary>
        /// Uppercase only.
        /// </summary>
        Upper=0x20,
        Operator=0x40,
        Enclosing=0x80,

        // Byte 2

        Quote=0x100,
        Escape=0x200,
        Separator=0x400,
        AlphaISO10646 = 0x800,

        HtmlTagSelectorStart=0x1000,
        HtmlTagSelectorExceptStart=0x2000,
        
        // A character that marks the end of an HTML tag opener (e.g. the end of the entire tag, or
        // the beginning of the attribute section)

        HtmlTagOpenerEnd=0x4000,
        HtmlTagAny=0x8000,

        // Byte 3

        HtmlTagNameStart = 0x10000,
        HtmlTagNameExceptStart = 0x20000,
        HtmlIDNameExceptStart = 0x40000,
        HtmlIDNameSelectorExceptStart=0x80000,

        // an HTML "space" is actually different from "white space" which is defined in the HTML5 spec
        // as UNICODE whitespace and is a lot of characters. But we are generally only concerned with
        // "space" characters which delimit parts of tags and so on.

        HtmlSpace = 0x100000,


        /// <summary>
        /// A character that must be HTML encoded to create valid HTML
        /// </summary>
        
        HtmlMustBeEncoded = 0x200000
    }
}
