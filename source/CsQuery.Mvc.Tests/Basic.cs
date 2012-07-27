using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuery.Mvc.Tests.Controllers;

namespace CsQuery.Mvc.Tests
{
    [TestClass]
    public class Basic: MvcTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var ctl = TestObjectFactory.ContextBoundController<TestController>("Test","LogOn");
            var result =  ctl.LogOn();
            result.ExecuteResult(ctl.ControllerContext);
        }
    }
}
