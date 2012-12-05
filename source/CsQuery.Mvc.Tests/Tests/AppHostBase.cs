using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Web.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuery.Mvc.Tests.Controllers;
using CsQuery.Mvc;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.Tests
{
    /// <summary>
    /// A base class that creates a private application host instance for tests that need to simulate
    /// a web hosting environment.
    /// </summary>

    [TestClass]
    public abstract class AppHostBase
    {

        public AppHostBase()
        {
            Initialize();
        }

        private void Initialize()
        {
            Host = MvcAppHost.CreateApplicationHost<MvcTestApp>();
        }

        protected CQ RenderView<T>(string action, bool destroyContext = false) where T : Controller, new()
        {
            return CQ.Create(Host.RenderView<T>(action,destroyContext));   
        }

        protected MvcAppHost Host;

        ~AppHostBase() {
            //Host.ClearContext();
            //Host.Dispose();
        }
         
    }
}
