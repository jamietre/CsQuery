using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.Implementation;

namespace CsQuery.Tests.Csharp.Dom
{
    [TestFixture, TestClass]
    public class TextReaderCombiner_
    {
        /// <summary>
        /// Issue #15
        /// </summary>
        [Test, TestMethod]
        public void CombineStringreaders()
        {
            string string1 = "Line1\nLine2";
            string string2 = "abcdef";

            var sc = new TextReaderCombiner(new StringReader(string1), new StringReader(string2));

            Assert.AreEqual(string1 + string2, sc.ReadToEnd());

            sc = new TextReaderCombiner(new StringReader(string1), new StringReader(string2));
            Assert.AreEqual("Line1",sc.ReadLine());
            Assert.AreEqual("Line2", sc.ReadLine());
            Assert.AreEqual("abcdef", sc.ReadLine());
            Assert.AreEqual(-1, sc.Peek());

        }
    }
}

