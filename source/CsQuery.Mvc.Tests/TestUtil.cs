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
        /// Renders the view output into a CQ object
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="action">
        /// The action.
        /// </param>
        ///
        /// <returns>
        /// a CQ object
        /// </returns>

        //public static CQ RenderViewCQ<T>(string action, bool destroyContext=true) where T : Controller, new()
        //{
        //    return CQ.CreateFragment(TestConfig.Host.RenderView<T>(action,destroyContext));
        //}

    }
}
