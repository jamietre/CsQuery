using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.EquationParser;
using CsQuery.EquationParser.Implementation;
using CsQuery.EquationParser.Implementation.Functions;
using CsQuery.Implementation;
using CsQuery.Engine;
using CsQuery.HtmlParser;

namespace CsQuery.Tests.Core.Dom
{
    [TestFixture, TestClass]
    public class SharedIndexTests<T>: CsQueryTest where T : IDomIndex, new()
    {
        protected IDomIndex Index;
        protected DomDocument Doc;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            CQ source = testHtml;

            Index = new T();
            Doc = new DomDocument(Index);
            
            Dom = new CQ(Doc);

            Doc.AppendChild(source["html"][0]);
        }

        public void RunAllTests()
        {
            QueryByID();
        }

        
        public void QueryByID()
        {
            Assert.AreEqual(1,Dom["#div1"].Length);
            Assert.AreEqual(Dom["#div1"],((IDomIndexSimple)Index).QueryIndex(GetKey("#div1")));

        }
        private ushort[] GetKey(string what)
        {
            var token = HtmlData.Tokenize(what.Substring(1));
            ushort[] key = new ushort[2] { what[0], token };
            return key;

        }
        private readonly string testHtml = @"
<html>
    <div id=div1> 
        <a class='links active'>
            <span data='something'> Some text </span>
        </a>
    </div>
    <div id=div2>
       <span> Some more text </span>
    </div>
</html>               
";

    }
}

