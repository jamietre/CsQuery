using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using CsQuery.Engine;

namespace CsQuery
{
    /// <summary>
    /// Global configuration and defaults
    /// </summary>

    public static class Config
    {
        #region constructor 
    
        static Config()
        {
            DynamicObjectType = typeof(JsObject);

        }
        #endregion

        #region private properties

        private static Type _DynamicObjectType;

        #endregion

        /// <summary>
        /// The default rendering options. These are flags.
        /// </summary>

        public static DomRenderingOptions DomRenderingOptions =
            DomRenderingOptions.QuoteAllAttributes;

        /// <summary>
        /// The default startup options. These are flags. 
        /// </summary>

        public static StartupOptions StartupOptions = StartupOptions.LookForExtensions;

        /// <summary>
        /// Default document type.
        /// </summary>
        
        public static DocType DocType = DocType.HTML5;

        /// <summary>
        /// Gets or sets the default dynamic object type. This is the type of object used by default when
        /// parsing JSON into an unspecified type.
        /// </summary>

        public static Type DynamicObjectType
        {
            get
            {
                return _DynamicObjectType;
            }
            set
            {
                if (value.GetInterfaces().Where(item =>
                    item == typeof(IDynamicMetaObjectProvider) ||
                    item == typeof(IDictionary<string, object>))
                    .Count() == 2)
                {
                    _DynamicObjectType = value;
                }
                else
                {
                    throw new ArgumentException("The DynamicObjectType must inherit IDynamicMetaObjectProvider and IDictionary<string,object>. Example: ExpandoObject, or the built-in JsObject.");
                }
            }
        }

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
