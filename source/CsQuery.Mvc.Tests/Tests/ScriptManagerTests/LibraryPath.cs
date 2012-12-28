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
    public class LibraryPath: AppHostBase
    {
        public LibraryPath()
            : base()
        {

        }

        [TestMethod]
        public void Default()
        {
            Assert.AreEqual(3, Host.LibraryPath.Count);
            CollectionAssert.AreEqual(new string[] { "~/scripts/lib/", "~/scripts/libs/", "~/scripts/libs2/" }, Host.LibraryPath.ToList());

        }

        [TestMethod]
        public void AutoRemoveMissing()
        {
            var doc = RenderView<TestController>("index",false);
            var libPath = Host.LibraryPath.ToList();

            Assert.AreEqual(2, Host.LibraryPath.Count);
            
            CollectionAssert.AreEqual(new string[] { "~/scripts/libs/", "~/scripts/libs2/" }, libPath);

        }
        [TestMethod]
        public void Normalizing()
        {
            PathList list = new PathList();
            list.Add("libs");
            list.Add("/libs2");
            list.Add("~/libs3");

            CollectionAssert.AreEqual(new string[] { "~/libs/", "~/libs2/", "~/libs3/"}, list.ToList());

        }
        [TestMethod]
        public void NormalizeName()
        {
            Assert.AreEqual("test.js", PathList.NormalizeName("test"));
            Assert.AreEqual("test.js", PathList.NormalizeName("test.js"));
            Assert.AreEqual("test.css", PathList.NormalizeName("test.css"));
            Assert.AreEqual("test.crap.js", PathList.NormalizeName("test.crap"));
            Assert.AreEqual("test.crap.css", PathList.NormalizeName("test.crap.css"));

            Assert.AreEqual("test.js", PathList.NormalizeName("~/test"));
            Assert.AreEqual("path/test.js", PathList.NormalizeName("path/test"));

            Assert.AreEqual("scripts/r2/foundation/jquery.foundation.navigation.js", PathList.NormalizeName("/scripts/r2/foundation/jquery.foundation.navigation.js?v=2302768"));
        }

        [TestMethod]
        public void GetName()
        {
            PathList list = new PathList();
            list.Add("libs");
            list.Add("/libs2");
            list.Add("~/libs3");

            Assert.AreEqual("test.js", list.GetName("test"));
            Assert.AreEqual("test.js", list.GetName("test.js"));
            Assert.AreEqual("test.js", list.GetName("~/test.js"));
            Assert.AreEqual("test.js", list.GetName("~/libs/test.js"));
            Assert.AreEqual("test.js", list.GetName("/libs/test.js"));
            Assert.AreEqual("test.js", list.GetName("~/libs3/test.js"));
            Assert.AreEqual("something/test.js", list.GetName("something/test.js"));
            Assert.AreEqual("something/test.js", list.GetName("~/libs2/something/test"));

        }
    }
}
