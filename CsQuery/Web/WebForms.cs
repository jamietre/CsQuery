using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using CsQuery.Web;

namespace CsQuery.Web
{
    public static class WebForms
    {
        public static CsQueryHttpContext CreateFromRender(Page page, Action<HtmlTextWriter> renderMethod, HtmlTextWriter writer)
        {
            return CreateFromRender(page, renderMethod, writer, HttpContext.Current);
        }


        /// <summary>
        /// Creates a new CSQuery object from a Page.Render method. The base Render method of a page should be overridden,
        /// and this called from inside it to configure the CsQUery
        /// </summary>
        /// <param name="page">The current System.Web.UI.Page</param>
        /// <param name="renderMethod">The delegate to the base render method</param>
        /// <param name="writer">The HtmlTextWriter to output the final stream (the parameter passed to the Render method)</param>
        public static CsQueryHttpContext CreateFromRender(
            Page page,
            Action<HtmlTextWriter> renderMethod,
            HtmlTextWriter writer,
            HttpContext context)
        {
            CsQueryHttpContext csqContext = new CsQueryHttpContext(context);
            csqContext.RealWriter = writer;
            csqContext.Page = page;
            csqContext.ControlRenderMethod = renderMethod;
            csqContext.Create();
            return csqContext;

        }
    }
}
