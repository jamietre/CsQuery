﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
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
    public class SharedIndexTests<T> where T : IDomIndex, new()
    {   
        protected CQ CreateDom()
        {
            CQ source = testHtml;

            var index  = new T();
            var doc = new DomDocument(index);
            
            var dom = new CQ(doc);

            doc.AppendChild(source["html"][0]);
            return dom;

        }

        public void RunAllTests()
        {
            QueryByID();
            QueryByClass();
            QueryByAttribute();
            QueryByNodeName();
            ManyTokensDoesNotCorruptIndex();
        }

        
        public void QueryByID()
        {
            var dom = CreateDom();

            string selector = "#div1";
            Assert.AreEqual(1, dom[selector].Length);
            CollectionAssert.AreEqual(
                dom[selector].ToList(),
                ((IDomIndexSimple)dom.Document.DocumentIndex).QueryIndex(GetKey(selector)).ToList()
            );

        }

        public void QueryByClass()
        {
            var dom = CreateDom();

            string selector = ".links";

            Assert.AreEqual(2, dom[selector].Length);
            CollectionAssert.AreEqual(
                dom[selector].ToList(),
                ((IDomIndexSimple)dom.Document.DocumentIndex).QueryIndex(GetKey(selector)).ToList()
            );
        }

        public void QueryByNodeName()
        {
            var dom = CreateDom();

            string selector = "span";
            Assert.AreEqual(3, dom[selector].Length);
            CollectionAssert.AreEqual(
                dom[selector].ToList(),
                ((IDomIndexSimple)dom.Document.DocumentIndex).QueryIndex(GetKey("+"+selector)).ToList()
            );
        }


        public void QueryByAttribute()
        {
            var dom = CreateDom();
            string selector = "[data]";
            string key = "data";

            Assert.AreEqual(2, dom[selector].Length);
            
            CollectionAssert.AreEqual(
                dom[selector].ToList(),
                ((IDomIndexSimple)dom.Document.DocumentIndex).QueryIndex(GetKey("!"+key)).ToList()
            );
        }

        public void Remove()
        {
            var dom = CreateDom();

            string selector="span";

            var len = dom[selector].Length;

            dom["[data=something]"].Remove();

            Assert.AreEqual(len - 1, dom[selector].Length);
            Assert.AreEqual(len - 1, ((IDomIndexSimple)dom.Document.DocumentIndex).QueryIndex(GetKey("+"+selector)).ToList());

        }

        public void ManyTokensDoesNotCorruptIndex()
        {
            // Parse and render multiple large documents with a lot of unique string tokens (in this case, class names).
            // This ensures that the index is not corrupted at the boundary that it was previously at ~65535 - 256 unique strings, 
            // at which point the index counter would overflow and wrap around.
            // Tests issue #204, #205, #189, #164.
            CQ doc1 = Html(35000, 1);
            CQ doc2 = Html(35000, 35001);
            CQ doc3 = Html(35000, 70001);
            
            Assert.That(doc1.Render(), Is.EqualTo(Html(35000, 1)));
            Assert.That(doc2.Render(), Is.EqualTo(Html(35000, 35001))); // At this point the index counter would wrap and, due to the storage method in HtmlData, throw an OutOfRangeException.
            Assert.That(doc3.Render(), Is.EqualTo(Html(35000, 70001))); // At this point the index would be corrupted, giving unpredictable results for new unique strings
        }

        private ulong[] GetKey(string what)
        {
            var token = HtmlData.Tokenize(what.Substring(1));
            var key = new [] { what[0], token };
            return key;

        }

        private static string Html(int uniqueClassCount, int startAt)
        {
            var strb = new StringBuilder("<html><head></head><body>");
            for (var i = 0; i < uniqueClassCount; i++)
            {
                strb.AppendFormat("<i class=\"{0}\"></i>", startAt + i);
            }
            strb.Append("</body></html>");
            return strb.ToString();
        }


        private readonly string testHtml = @"
<html>
    <div id=div1 data='container'> 
        <a class='links active'>
            <span data='something'> Some text </span>
        </a>
    </div>
    <div id=div2>
       <span> Some more text </span>
    </div>
    <a class='links'>
        <span>2nd Links Text</span>
    </a>
</html>               
";

    }
}

