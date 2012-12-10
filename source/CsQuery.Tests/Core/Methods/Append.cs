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

namespace CsQuery.Tests.Core
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
        public void Append_AcrossDocuments()
        {
            
            var div = CQ.Create("<div></div>")
               .AddClass("admin-content");

            var span = CQ.Create("<span></span>")
                .AddClass("admin-content-body");

            div.Append(span);

            Assert.AreEqual(1, span.Length);

            
            Assert.AreEqual(div["span"][0], span[0]);
        }



   }
}