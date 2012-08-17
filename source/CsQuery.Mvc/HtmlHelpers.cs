using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace CsQuery.Mvc
{
    /// <summary>
    /// HTML helpers for CsQuery.MVC
    /// </summary>

    public static class HtmlHelpers
    {
        /// <summary>
        /// Include an HTML file server-side
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="helper">
        /// The context
        /// </param>
        /// <param name="serverPath">
        /// Relative path of the server file.
        /// </param>
        ///
        /// <returns>
        /// An HTML string of the included file
        /// </returns>

        public static IHtmlString ServerSideInclude<T>(this HtmlHelper<T> helper, string serverPath)
        {

            var filePath = HttpContext.Current.Server.MapPath(serverPath);

            // load from file
            var streamReader = File.OpenText(filePath);
            var markup = streamReader.ReadToEnd();
            streamReader.Close();

            var dom = CQ.Create(markup);
            return new HtmlString(markup);
        }

        /// <summary>
        /// Creates HTML for output using CsQuery.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="helper">
        /// The helper context.
        /// </param>
        /// <param name="html">
        /// A string of valid HTML, or a single tag name, e.g "div".
        /// </param>
        ///
        /// <returns>
        /// A new CsQuery object.
        /// </returns>

        public static CQ HtmlTag<T>(this HtmlHelper<T> helper, string html)
        {
            if (html.Contains(" "))
            {
                return new CQ(html);
            }
            else
            {
                return new CQ("<" + html + " />");
            }
        }
    }
}
