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

        [Test, TestMethod]
        public void Append_AcrossDocuments()
        {
            
            var div = CQ.Create("<div></div>")
               .AddClass("admin-content");

            var editor = CQ.Create("<div></div>")
                .AddClass("admin-content-body");

            div.Append(editor);

            Assert.AreEqual(1, editor.Length);
          
        }



   }
}