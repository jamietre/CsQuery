using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using CsQuery.Web;

namespace CsQuery.Web
{
    /// <summary>
    /// Utility class for integrating CsQuery with ASP.NET Webforms.
    /// </summary>

    public static class WebForms
    {
        /// <summary>
        /// Creates a new CsQueryHttpContext object bound to an ASP.NET WebForms page.
        /// </summary>
        ///
        /// <param name="page">
        /// The current System.Web.UI.Page.
        /// </param>
        /// <param name="renderMethod">
        /// The delegate to the base render method.
        /// </param>
        /// <param name="writer">
        /// The HtmlTextWriter to output the final stream (the parameter passed to the Render method)
        /// </param>
        ///
        /// <returns>
        /// A context which can be used to complete the Render after any manipulation with CsQuery.
        /// </returns>

        public static CsQueryHttpContext CreateFromRender(Page page, Action<HtmlTextWriter> renderMethod, HtmlTextWriter writer)
        {
            return CreateFromRender(page, renderMethod, writer, HttpContext.Current);
        }

        /// <summary>
        /// Creates a new CSQuery object from a Page.Render method. The base Render method of a page
        /// should be overridden, and this called from inside it to configure the CsQUery.
        /// </summary>
        ///
        /// <param name="page">
        /// The current System.Web.UI.Page.
        /// </param>
        /// <param name="renderMethod">
        /// The delegate to the base render method.
        /// </param>
        /// <param name="writer">
        /// The HtmlTextWriter to output the final stream (the parameter passed to the Render method)
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        ///
        /// <returns>
        /// A context which can be used to complete the Render after any manipulation with CsQuery.
        /// </returns>

        public static CsQueryHttpContext CreateFromRender(
            Page page,
            Action<HtmlTextWriter> renderMethod,
            HtmlTextWriter writer,
            HttpContext context)
        {
            return new CsQueryHttpContext(context, page, writer, renderMethod);

        }
    }
}
