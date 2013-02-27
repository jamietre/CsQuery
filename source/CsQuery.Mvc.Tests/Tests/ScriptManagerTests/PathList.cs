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
using CsQuery.Mvc.ClientScript;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.Tests
{
    [TestClass]
    public class PathList_
    {


        [TestMethod]
        public void CombinePath()
        {
            Assert.AreEqual("abc/def", PathList.CombinePath("abc","def"));
            Assert.AreEqual("abc/def", PathList.CombinePath("abc/","def"));
            Assert.AreEqual("abc\\def\\ghi", PathList.CombinePath("abc\\def", "ghi"));
            Assert.AreEqual("abc\\def\\ghi\\jkl", PathList.CombinePath("abc\\def", "ghi/jkl"));
            
            Assert.AreEqual("/abc/def", PathList.CombinePath("a", "/abc", "def"));
            Assert.AreEqual("~/abc/def", PathList.CombinePath("a", "~/abc", "def"));

            Assert.AreEqual("~/", PathList.CombinePath("~/"));
            Assert.AreEqual("\\", PathList.CombinePath("a","\\"));

        }

       
    }
}
