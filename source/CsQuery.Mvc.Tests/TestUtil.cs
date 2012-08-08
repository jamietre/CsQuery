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
        public static CQ RenderViewCQ<T>(string action) where T : Controller, new()
        {
            return CQ.CreateFragment(TestConfig.Host.RenderView<T>(action));
        }
    }
}
