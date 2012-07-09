using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Engine;

namespace CsQuery
{
    /// <summary>
    /// Global configuration and defaults
    /// </summary>

    public static class Config
    {

        /// <summary>
        /// The default rendering options. These are flags.
        /// </summary>
        public static DomRenderingOptions DomRenderingOptions = DomRenderingOptions.QuoteAllAttributes;

        /// <summary>
        /// Default document type.
        /// </summary>
        public static DocType DocType = DocType.HTML5;

        /// <summary>
        /// Provides access to the PseudoSelectors object, which allows registering new filters and accessing information
        /// and instances about existing filters.
        /// </summary>
        ///
        /// <value>
        /// The pseudo PseudoSelectors configuration object.
        /// </value>

        public static PseudoSelectors PseudoClassFilters
        {
            get {
                return PseudoSelectors.Items;
            }
        }
        
    }
}
