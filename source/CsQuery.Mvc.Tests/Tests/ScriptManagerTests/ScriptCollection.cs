using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Mvc.Tests.Controllers;
using CsQuery.Mvc;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;
using CsQuery.Mvc.ClientScript;

namespace CsQuery.Mvc.Tests.ScriptManagerTests
{
    [TestFixture,TestClass]
    public class ScriptCollectionTest
    {
        public ScriptCollectionTest()
        {
            var html = Support.GetFile("CsQuery.Mvc.Tests/Views/Test/unresolvedscripts.cshtml");
            Dom = CQ.Create(html);
            pathList = new PathList();
            pathList.Add("~/scripts");
        }

        CQ Dom;
        PathList pathList;

        [Test,TestMethod]
        public void SimpleTest() {
            ScriptCollection coll = new ScriptCollection(new ScriptEnvironment {
                RelativePathRoot="c:/",
                 LibraryPath=pathList, 
                 MapPath = MapPath
            });

        }

        private static string MapPath(string path)
        {
            return Support.GetFilePath(Support.CombinePaths("CsQuery.Mvc.Tests/",path));
        }
    }
}
