using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace CsQuery.Utility
{
    /// <summary>
    /// Provides information about the web browser used to access the current page (from HttpContext)
    /// </summary>

    public class Browser
    {
        /// <summary>
        /// Create a new instance from an HttpContext
        /// </summary>
        ///
        /// <param name="context">
        /// The context.
        /// </param>

        public Browser(HttpContext context)
        {
            Context = context;
            
        }

        private HttpContext Context;

        /// <summary>
        /// When true, indicates that the browser is Microsoft Internet Explorer of any version.
        /// </summary>

        public bool MSIE
        {
            get {
                return Context.Request.Browser.Browser == "IE";
            }
        }

        /// <summary>
        /// When true, indicates that the browser is Google Chrome of any version.
        /// </summary>

        public bool Chrome
        {
            get
            {
                return Context.Request.Browser.Browser == "Chrome";
            }
        }

        /// <summary>
        /// Gets the complete version number of the browser
        /// </summary>

        public string Version {
            get {
                return Context.Request.Browser.Version;
            }
        }

        /// <summary>
        /// Gets the major version number of the browsers.
        /// </summary>

        public int VersionMajor
        {
            get {
                return Context.Request.Browser.MajorVersion;
            }
        }

    }
}
