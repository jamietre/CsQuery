using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery;
using CsQuery.StringScanner;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Output
{
    /// <summary>
    /// Removes all extraneous whitespace
    /// </summary>
    public class OutputFormatterDefault: OutputFormatterBase
    {
        /// <summary>
        /// Create an output formatter using the default HTML encoder and default options.
        /// </summary>

        public OutputFormatterDefault()
            : base(DomRenderingOptions.Default, HtmlEncoders.Default)
        { }
        
        /// <summary>
        /// Create a new formatter using the specified HtmlEncoder.
        /// </summary>
        ///
        /// <param name="encoder">
        /// The encoder.
        /// </param>

        public OutputFormatterDefault(DomRenderingOptions options, IHtmlEncoder encoder)
            : base(options, encoder)
        {
        }

      
    }
}
