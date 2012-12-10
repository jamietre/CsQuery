using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;

namespace CsQuery.Tests.Core
{
    public partial class Methods: CsQueryTest
    {
        Array expected;

        public override void FixtureSetUp()
        {
 	        base.FixtureSetUp();

            Dom = TestDom("TestHtml");
            expected  =Arrays.Create(
                Dom["[title='2 silver badges']"][0],
                Dom["#hlinks-user"][0],
                Dom["body"][0],
                Dom["html"][0]
            );
        }

        [Test, TestMethod]
        public void ParentsUntil()
        {
            var res = Dom[".badge2"].ParentsUntil();
            CollectionAssert.AreEqual(expected, res.Elements);

        }

        [Test, TestMethod]
        public void ParentsUntil_Element()
        {

            var res = Dom[".badge2"].ParentsUntil("body");
            CollectionAssert.AreEqual(expected.Slice(0,2), res);

        }

        [Test, TestMethod]
        public void ParentsUntil_Sequence()
        {

            var res = Dom[".badge2"].ParentsUntil("[id]");

            // will stop at the first element with an ID, only the immediate parent has no id
            CollectionAssert.AreEqual(expected.Slice(0, 1), res);

        }
        [Test, TestMethod]
        public void ParentsUntil_Filtered()
        {

            var res = Dom[".badge2"].ParentsUntil("body","[id]");

            // will only match the hlinks-user one (nothing else has an ID)
            // 
            CollectionAssert.AreEqual(expected.Slice(1,2), res);

        }
    }
}