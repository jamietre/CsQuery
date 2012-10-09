using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using CsQuery.Engine;
using CsQuery.Output;

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
            _DomRenderingOptions =   DomRenderingOptions.QuoteAllAttributes;
            HtmlEncoder = HtmlEncoders.Basic;
        }

        #endregion

        #region private properties

        /// <summary>
        /// Internal to avoid Obsolete warning from DomRenderingOptions until we remove it
        /// </summary>

        internal static DomRenderingOptions _DomRenderingOptions;
        private static Type _DynamicObjectType;
        
        private static IOutputFormatter GetOutputFormatter()
        {

            return OutputFormatters.Default;
        }

        #endregion

        #region public properties

        /// <summary>
        /// The default rendering options. These will be used when configuring a default OutputFormatter.
        /// Note that if the default OutputFormatter has been changed, this setting is not guaranteed to
        /// have any effect on output.
        /// </summary>

        public static DomRenderingOptions DomRenderingOptions  
        {
            get {
                return _DomRenderingOptions;
            }
            set {
                _DomRenderingOptions = value;
            }
        }

        /// <summary>
        /// The default HTML encoder.
        /// </summary>

        public static IHtmlEncoder HtmlEncoder
        {
            get;
            set;
        }

        /// <summary>
        /// A delegate that returns a new instance of the default output formatter to use for rendering
        /// when none is otherwise specified.
        /// </summary>

        public static Func<IOutputFormatter> OutputFormatter;


        /// <summary>
        /// The default startup options. These are flags. 
        /// </summary>

        public static StartupOptions StartupOptions = StartupOptions.LookForExtensions;

        /// <summary>
        /// Default document type. This is the parsing mode that will be used when creating documents
        /// that have no DocType and no mode is explicitly defined.
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
        #endregion

    }
}
