using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;

namespace CsQuery.Tests.Csharp
{
    /// <summary>
    /// This method is largely covered by the jQuery tests
    /// </summary>

    public partial class Methods: CsQueryTest
    {
        /// <summary>
        /// When adding something from one document to another, the selection set should remain unchanged,
        /// and refer to elements in the target document.
        /// </summary>

        [Test, TestMethod]
        public void Select_ExpandTags()
        {
            
            var div = CQ.Create("<div/><input/><span/>");
            Assert.AreEqual("<div><input /><span></span></div>", div.Render());

            // this should permit the self-closing div and not nest the other stuff
            var div2 = div["<div/><input/><span />"];

            Assert.AreEqual("<div></div><input /><span></span>",div2.RenderSelection());


        }



   }
}