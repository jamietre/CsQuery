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
using CsQuery.Mvc.ClientScript;

namespace CsQuery.Mvc.Tests
{
    [TestClass]
    public class UsingOptions
    {

        [TestMethod]
        public void OptionParsing()
        {

            PathList list = new PathList();
            list.Add("~/scripts/libs");
            list.Add("~/scripts/libs2");

            var coll = new ScriptCollection(new ScriptEnvironment {
                RelativePathRoot= "/",
                LibraryPath=  list, 
                MapPath = TestUtil.MapPath
            });

            coll.AddPath("~/scripts/test-script-3.js");

            Assert.AreEqual(coll.Count, 1);

        }
 
    }
}
