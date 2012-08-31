using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.Tests
{
    public class TestUtil
    {

        /// <summary>
        /// Like HttpContext.MapPath, except in the test context
        /// </summary>
        ///
        /// <param name="path">
        /// Relative path to a file
        /// </param>
        ///
        /// <returns>
        /// A hard filesystem path
        /// </returns>

        public static string MapPath(string path)
        {
            if (path.StartsWith("~/"))
            {
                path = path.Substring(2);
            }
            else if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            path = path.Replace("/", "\\");
            return Path.Combine(TestConfig.AppPath, path);
        }
        

    }
}
