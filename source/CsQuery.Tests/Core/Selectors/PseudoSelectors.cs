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

namespace CsQuery.Tests.Core.Selectors
{
    /// <summary>
    /// Base class for PseudoSelector tests
    /// </summary>

    [TestFixture, TestClass]
    public abstract class PseudoSelector : CsQueryTest
    {
        /// <summary>
        /// Used by all tests in the partial class. This happens to be the first one we set up. 
        /// </summary>

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            Dom = TestDom("testhtml");
        }



        protected CQ VisibilityTestDom()
        {
            return CQ.Create(@"<div id='wrapper'>
                    <div id='outer' style='display:none;'>
                    <span id='inner'>should be hidden</span></div>
                    <div id='outer2' width='10'><span id='inner2'>should not be hidden</span></div>
                    <div id='outer3' height='0'><span id='inner3'>hidden</span></div>
                    <div id='outer4' style='width:0px;'><span id='inner4'>hidden</span></div>
                    <div id='outer5' style='display:block'><span id='inner5'>visible</span></div>
                    <div id='outer6' style='opacity: 0;'>Hidden</div>
                    <input type='hidden' value='nothing'>
                </div>
            ");

        }
    }
}