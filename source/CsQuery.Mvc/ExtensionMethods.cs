using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Extension methods that support MVC integration
    /// </summary>

    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns the complete HTML-encoded string of the selection set. 
        /// </summary>
        ///
        /// <returns>
        /// An IHtmlString object
        /// </returns>
        public static IHtmlString AsHtmlString(this CQ dom)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IDomObject elm in dom)
            {
                elm.Render(sb);
            }
            return new HtmlString(sb.ToString());
        }

        public static bool Any<T>(this IEnumerable list, Func<T,bool> predicate) 
        {
            foreach (var item in list)
            {
                if (item is T && predicate((T)item))
                {
                    return true;
                }
            }
            return false;
        }
    
    }
}
