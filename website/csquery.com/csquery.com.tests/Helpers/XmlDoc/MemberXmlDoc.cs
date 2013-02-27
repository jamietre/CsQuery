using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsQuerySite;
using CsQuerySite.Helpers.XmlDoc;

namespace csquery.com.tests
{
    [TestClass]
    public class MemberXmlDocTest
    {
        [TestMethod]
        public void Constructor()
        {
        }
        [TestMethod]
        public void Properties()
        {
            var mi  =typeof(MemberXmlDoc).GetMember("Signature");

            Assert.AreEqual(1,mi.Length);
            var xd = new MemberXmlDoc(mi[0]);

            Assert.AreEqual(xd.IsStatic, false);


            string contents = "\r\n             The signature is built from reflected info. The signature returns formatted code that\r\n             should match the compilable method signature.\r\n             ";

            Assert.AreEqual(contents,xd["remarks"]);

        }


    }
}
