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
    [TestClass]
    public class ScriptLocations: AppHostBase
    {
        public ScriptLocations()
            : base()
        {
            Doc = RenderView<TestController>("index4", false);
        }
        
        CQ Doc;

        [TestMethod]
        public void VerifyLocations()
        {
            var dom = Doc["head > script"];
            // 2 that are there already; 1 moved from body; 1 that is created by bundler.
            
            Assert.AreEqual(4, dom.Length);

            dom = Doc["body > script"];
            Assert.AreEqual(2, dom.Length);
            Assert.AreEqual("inbody", dom.Attr("class"));



        }
         
    }
}
