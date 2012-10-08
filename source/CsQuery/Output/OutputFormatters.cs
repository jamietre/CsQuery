using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Output;

namespace CsQuery
{
    public static class OutputFormatters
    {
        /// <summary>
        /// Creates an instance of the default OutputFormatter using the options passed.
        /// </summary>
        ///
        /// <param name="options">
        /// (optional) options for controlling the operation.
        /// </param>
        /// <param name="encoder">
        /// (optional) the encoder.
        /// </param>
        ///
        /// <returns>
        /// An OutputFormatter.
        /// </returns>

        public static IOutputFormatter Create(DomRenderingOptions options, IHtmlEncoder encoder)
        {
            return new OutputFormatterDefault(options, encoder ?? HtmlEncoders.Default);
        }

        /// <summary>
        /// Creates an instance of the default OutputFormatter using the options passed and the default encoder.
        /// </summary>
        ///
        /// <param name="options">
        /// (optional) options for controlling the operation.
        /// </param>
        ///
        /// <returns>
        /// An OutputFormatter.
        /// </returns>

        public static IOutputFormatter Create(DomRenderingOptions options)
        {
            return new OutputFormatterDefault(options, HtmlEncoders.Default);
        }

        /// <summary>
        /// Creates an instance of the default OutputFormatter using the default options and the encoder
        /// passed. 
        /// </summary>
        ///
        /// <param name="options">
        /// (optional) options for controlling the operation.
        /// </param>
        ///
        /// <returns>
        /// An OutputFormatter.
        /// </returns>

        public static IOutputFormatter Create(IHtmlEncoder encoder)
        {
            return new OutputFormatterDefault(DomRenderingOptions.Default,encoder);
        }
        /// <summary>
        /// Gets an instance of the default OuputFormatter
        /// </summary>

        public static IOutputFormatter Default 
        {
            get {
                return new OutputFormatterDefault();
            }
        }

        
        /// <summary>
        /// Merge options with defaults when needed
        /// </summary>
        ///
        /// <param name="options">
        /// (optional) options for controlling the operation.
        /// </param>

        private static void MergeOptions(ref DomRenderingOptions options)
        {
            if (options.HasFlag(DomRenderingOptions.Default))
            {
                options = CsQuery.Config._DomRenderingOptions | options & ~(DomRenderingOptions.Default);
            }
        }
    }
}
