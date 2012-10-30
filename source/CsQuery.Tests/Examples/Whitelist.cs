using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;
using CsQuery.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using System.Diagnostics;

//namespace CsQuery.Tests._Examples
//{
//    /// <summary>
//    /// This test is disabled by default because it accesses public web sites, activate it just to test this feature
//    /// </summary>
//    [TestFixture, TestClass]
//    public class WhiteList : CsQueryTest
//    {

//        private string unsafeHtml = @"</p> 
//            Hello there, just wondering if your room is still free?<br>I would in the house.<br>
//            I have trained in reflexology and will </a>be returning to college soon to train in massage.<br>
//            I am looking for a room in a house that I can call home.  I am ready to move in whenever suits.<br>
//            Really looking forward to hearing from you.<br>Cheers Lollie<p>";

//        [Test,TestMethod]
//        public void BadOpeningHtml()
//        {
//            CQ doc = CQ.CreateFragment(unsafeHtml);
//            var not = doc.Not("p,br"); // Problem is not includes all the text nodes which I want to keep. Any ideas?
//            not.Unwrap();
//            var safeHtml = doc.Render();

//            Assert.AreEqual("", safeHtml);
//        }
       
//    }
//}
