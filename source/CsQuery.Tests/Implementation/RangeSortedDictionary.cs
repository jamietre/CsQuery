using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.Implementation;

namespace CsQuery.Tests.Core.Implementation
{
    [TestFixture, TestClass]
    public class RangeSortedDictionary_: CsQueryTest
    {
        Random rnd = new Random(2);
        char[] allChars;
        

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            allChars = new char[65536];
            for (int i = 0; i < 65536; i++)
            {
                allChars[i] = (char)i;
            }
        }

        protected char[] RandomizedChars()
        {
            return allChars.OrderBy(x => rnd.Next()).ToArray();
        }

        private static char[] chars;
        private static int pos = 0;

        protected string NextKey(int length)
        {
            string key= "";
            if (chars == null)
            {
                chars = RandomizedChars();
            }

            int last = pos + length;
            
            for (int i = pos; i < last; i++)
            {
                key += chars[i % 65536];
            }
            if (last > 65535)
            {
                chars = RandomizedChars();
                pos= 0;
            } else {
                pos = last;
            }
            return key;
        }
        protected string RandomKey(int length)
        {
            string key = "";
            for (int i = 0; i < length; i++)
            {
                key += allChars[rnd.Next(65536)];
            }
            return key;
        }


        /// <summary>
        /// Ensure that sort order is correct
        /// </summary>
        [Test, TestMethod]
        public void Sorting()
        {
            var rand = RandomizedChars();
            Assert.AreNotEqual(rand, allChars);
            
            var sorted = rand.OrderBy(item => item);
            Assert.AreEqual(sorted, allChars);
        }
    }
}

