using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuery.Mvc.Tests.Controllers;
using CsQuery.Mvc;

namespace CsQuery.Mvc.Tests
{
    [TestClass]
    public class ScriptManager
    {
        [TestMethod]
        public void Scripts_Includ()
        {
            var doc = TestUtil.RenderViewCQ<TestController>("index");

        }

    }
}
