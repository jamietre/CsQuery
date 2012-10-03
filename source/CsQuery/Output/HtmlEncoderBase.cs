using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery.StringScanner;

namespace CsQuery.Output
{
    /// <summary>
    /// Abstract base class for custom HTML encoder implementations
    /// </summary>

    public class HtmlEncoderBase: IHtmlEncoder
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

        protected virtual bool TryEncode(char c, out string encoded);

        /// <summary>
        /// Encodes text as HTML, writing the processed output to the TextWriter.
        /// </summary>
        ///
        /// <param name="html">
        /// The text to be encoded.
        /// </param>
        /// <param name="output">
        /// The target for the ouput.
        /// </param>

        public virtual void Encode(string html, TextWriter output)
        {
            StringBuilder sb = new StringBuilder();
            int pos = 0,
                len = html.Length;

            while (pos < len)
            {
                char c = html[pos++];
                string encoded;
                if (TryEncode(c, out encoded))
                {
                    output.Write(encoded);
                }
                else
                {
                    output.Write(c);
                }
            }
        }

        /// <summary>
        /// Decodes a string of HTML to text.
        /// </summary>
        ///
        /// <param name="value">
        /// The HTML to be decoded.
        /// </param>
        /// <param name="output">
        /// The target for the ouput.
        /// </param>

        public virtual void Decode(string value, TextWriter output)
        {
            System.Web.HttpUtility.HtmlDecode(value, output);
        }
    }
}
