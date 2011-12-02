using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtc.CsQuery;

namespace CsqueryTests.Csharp
{
    [TestClass]
    public class Rendering
    {

        protected string node = "<div class='a b c c' attr1='{\"somejson\": \"someval\"}'";



        [TestMethod]
        public void HtmlCleanup()
        {
            var dom = CsQuery.Create(node);
            var expected =  "<div class=\"a b c\" attr1='{\"somejson\": \"someval\"}'></div>";
            Assert.AreEqual(expected, dom.Render(), "Basic cleanup - no duplicated class - missing end tag");


            // TODO
            // test attribute rendering options
            // Doctype options

        }
        [TestMethod]
        public void AttributeQuoting()
        {


        }
    }
}
