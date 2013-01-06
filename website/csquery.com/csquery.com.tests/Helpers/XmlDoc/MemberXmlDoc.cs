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

        public void Properties()
        {
            var mi  =typeof(MemberXmlDoc).GetMember("Signature");

            Assert.AreEqual(1,mi.Length);
            var xd = new MemberXmlDoc(mi[0]);

            Assert.AreEqual(xd.IsStatic, false);


        }
    }
}
