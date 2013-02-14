using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CsQuery.Mvc.ClientScript;

namespace CsQuery.Mvc
{
    /// <summary>
    /// HTML helpers for CsQuery.MVC
    /// </summary>

    public static class HtmlHelpers
    {
        /// <summary>
        /// Include an HTML file server-side.
        /// </summary>
        ///
        /// <param name="helper">
        /// The context.
        /// </param>
        /// <param name="serverPath">
        /// Relative path of the server file.
        /// </param>
        ///
        /// <returns>
        /// An HTML string of the included file.
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
        /// Includes a Javascript file inline.
        /// </summary>
        ///
        /// <param name="helper">
        /// The current helper context.
        /// </param>
        /// <param name="script">
        /// Full pathname of the server file.
        /// </param>
        /// <param name="attributes">
        /// (optional) the attributes.
        /// </param>
        /// <param name="location">
        /// (optional) The location of the script. This parameter can be used to move the script into the
        /// &lt;head&gt; tag of the document.
        /// </param>
        ///
        /// <returns>
        /// An HtmlString.
        /// </returns>

        public static IHtmlString Script<T>(this HtmlHelper<T> helper, string script, object attributes, ScriptLocations location)
        {
            string path = PathList.NormalizePath(PathList.NormalizeName(script));

            var el = CQ.CreateFragment("<script />");

            if (attributes != null)
            {
                el.AttrSet(attributes);
            }
            if (!el.HasAttr("type"))
            {
                el.Attr("type", "text/javascript");
            }
            el.Attr("src", script + 
                (script.EndsWith(".js",StringComparison.CurrentCultureIgnoreCase) ? "" : ".js")
            );
            
            if (location == ScriptLocations.Head)
            {
                el.Attr("data-moveto","head");
            }

            return new HtmlString(el.Render());
        }

        /// <summary>
        /// Includes a Javascript file inline.
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
        /// <param name="attributes">
        /// (optional) the attributes.
        /// </param>
        ///
        /// <returns>
        /// An HtmlString.
        /// </returns>

        public static IHtmlString Script<T>(this HtmlHelper<T> helper, string script, object attributes)
        {
            return Script(helper, script, attributes, ScriptLocations.Inline);
        }

        /// <summary>
        /// Includes a Javascript file inline.
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
        /// <param name="location">
        /// (optional) The location of the script. This parameter can be used to move the script into the
        /// &lt;head&gt; tag of the document.
        /// </param>
        ///
        /// <returns>
        /// An HtmlString.
        /// </returns>

        public static IHtmlString Script<T>(this HtmlHelper<T> helper, string script, ScriptLocations location)
        {
            return Script(helper, script, null, location);
        }

        /// <summary>
        /// Includes a Javascript file inline.
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
            return Script(helper, script, null, ScriptLocations.Inline);
        }
        
    }
}
