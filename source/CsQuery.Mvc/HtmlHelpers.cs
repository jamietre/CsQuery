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

        public static CqHtmlString HtmlTag<T>(this HtmlHelper<T> helper, string html)
        {
            if (html.Contains(" "))
            {
                return new CqHtmlString(html);
            }
            else
            {
                return new CqHtmlString("<" + html + " />");
            }
        }

        /// <summary>
        /// Includes a server-side javascript file inline.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="helper">
        /// The current helper context.
        /// </param>
        /// <param name="script">
        /// Full pathname of the server file.
        /// </param>
        ///
        /// <returns>
        /// An HtmlString.
        /// </returns>

        public static IHtmlString Script<T>(this HtmlHelper<T> helper, string script)
        {
            return new HtmlString(String.Format("<script class=\"csquery-script\" type=\"text/javascript\" src=\"{0}\" ></script>",script));
        }

        /// <summary>
        /// Sets the script root for this page
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="helper">
        /// The current helper context.
        /// </param>
        /// <param name="rootPath">
        /// Full pathname of the root file.
        /// </param>
        ///
        /// <returns>
        /// .
        /// </returns>

        public static IHtmlString ScriptRoot<T>(this HtmlHelper<T> helper, string rootPath)
        {
            return new HtmlString(String.Format("<script class=\"csquery-script\" csquery root=\"{0}\" ></script>",rootPath));
        }
    }
}
