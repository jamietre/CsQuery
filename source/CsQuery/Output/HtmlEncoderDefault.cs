using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CsQuery.Output
{
    /// <summary>
    /// Default HTML encoder. This parses less-than, greater-than, ampersand, double-qoute, and non-
    /// breaking space, plus all characters above ascii 160 into ther HTML coded equivalent.
    /// </summary>

    public class HtmlEncoderDefault: HtmlEncoderBase
    {
        /// <summary>
        /// Determines of a character must be encoded; if so, encodes it as the output parameter and
        /// returns true; if not, returns false.
        /// </summary>
        ///
        /// <param name="c">
        /// The text string to encode.
        /// </param>
        /// <param name="encoded">
        /// [out] The encoded string.
        /// </param>
        ///
        /// <returns>
        /// True if the character was encoded.
        /// </returns>

        protected override bool TryEncode(char c, out string encoded)
        {
            switch (c)
            {
                case '<':
                    encoded = "&lt;";
                    return true;
                case '>':
                    encoded = "&gt;";
                    return true;
                case '"':
                    encoded = "&quot;";
                    return true; ;
                case '&':
                    encoded = "&amp;";
                    return true; ;
                case (char)160:
                    encoded = "&nbsp;";
                    return true; ;
                default:
                    if (c > 160)
                    {
                        // decimal numeric entity
                        encoded = EncodeNumeric(c);
                        return true;
                    }
                    else
                    {
                        encoded = null;
                        return false;
                    }
            }
        }
        protected override bool TryEncodeAstralPlane(int c, out string encoded)
        {
            encoded = EncodeNumeric(c);
            return true;
        }

        protected string EncodeNumeric(int value)
        {
            return "&#" + (value).ToString(System.Globalization.CultureInfo.InvariantCulture) + ";";
        }
    }
}
