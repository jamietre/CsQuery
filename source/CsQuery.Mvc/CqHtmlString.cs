using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Subclass of CQ that simply implements IHtmlString. The "ToHtmlString" method is already implemented
    /// by the main object; but adding this interface to creates a requirement for a reference to System.Web
    /// in non-MVC projects. We use this pass-through subclass to avoid that.
    /// </summary>

    public class CqHtmlString : CQ, IHtmlString
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public CqHtmlString()
            : base()
        {

        }

        /// <summary>
        /// Create a CqHtmlString from HTML
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>

        public CqHtmlString(string html): base(html)
        {

        }

        /// <summary>
        /// Creates a new instance of the CqHtmlString object.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>

        protected override CQ NewCqUnbound()
        {
            return new CqHtmlString();
        }

        /// <summary>
        /// Returns the complete HTML-encoded string of the selection set.
        /// </summary>
        ///
        /// <returns>
        /// An HTML-encoded string.
        /// </returns>

        public string ToHtmlString()
        {
            return RenderSelection();
        }
    }
}
