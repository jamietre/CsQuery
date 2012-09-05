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
            CollectionAssert.AreEqual(new string[] { "~/scripts/lib", "~/scripts/libs", "~/scripts/libs2" }, Host.LibraryPath.ToList());

        }

        [TestMethod]
        public void AutoRemoveMissing()
        {
            var doc = RenderView<TestController>("index",false);
            var libPath = Host.LibraryPath.ToList();

            Assert.AreEqual(2, Host.LibraryPath.Count);
            
            CollectionAssert.AreEqual(new string[] { "~/scripts/libs", "~/scripts/libs2" }, libPath);

        }
        [TestMethod]
        public void Normalizing()
        {
            PathList list = new PathList();
            list.Add("libs");
            list.Add("/libs2");
            list.Add("~/libs3");

            CollectionAssert.AreEqual(new string[] { "~/libs", "/libs2", "~/libs3"}, list.ToList());

        }
         
    }
}
