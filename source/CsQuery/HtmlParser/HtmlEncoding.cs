using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser
{
    /// <summary>
    /// Simplify access to character set encodings for this system.
    /// </summary>

    public static class HtmlEncoding
    {
        private static Dictionary<string, EncodingInfo> _Encodings;

        /// <summary>
        /// A dictionary of all encodings available on this system
        /// </summary>

        private static Dictionary<string, EncodingInfo> Encodings
        {
            get
            {
                if (_Encodings == null)
                {
                    _Encodings = new Dictionary<string, EncodingInfo>(StringComparer.CurrentCultureIgnoreCase);
                    foreach (var encoding in Encoding.GetEncodings())
                    {
                        _Encodings[encoding.Name] = encoding;
                    }
                }
                return _Encodings;
            }
        }

        /// <summary>
        /// Try to get a character set encoding from its web name.
        /// </summary>
        ///
        /// <param name="encodingName">
        /// Name of the encoding.
        /// </param>
        /// <param name="encoding">
        /// [out] The encoding.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        public static bool TryGetEncoding(string encodingName, out Encoding encoding)
        {
            EncodingInfo info;
            if (Encodings.TryGetValue(encodingName, out info)) {
                encoding = info.GetEncoding();
                return true;
            } else {
                encoding =null;
                return false;
            }
        }

        /// <summary>
        /// Gets an encoding.
        /// </summary>
        ///
        /// <param name="encodingName">
        /// Name of the encoding.
        /// </param>
        ///
        /// <returns>
        /// The encoding.
        /// </returns>

        public static Encoding GetEncoding(string encodingName)
        {
            Encoding encoding;
            if (TryGetEncoding(encodingName, out encoding)) {
                return encoding;
            } else {
                return null;
            }
        }
    }
}
