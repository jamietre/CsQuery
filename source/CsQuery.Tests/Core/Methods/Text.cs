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

        [Test, TestMethod]
        public void TextFunc()
        {

            var dom = TestDom("TestHtml");

            Assert.AreEqual("jamietre",dom[".profile-link"].Text());
            dom["*"].Text(SetText);

            Assert.AreEqual("user:jamietre", dom[".profile-link"].Text());
            Assert.AreEqual("3,215",dom[".reputation-score"].Text());
        }

        private string SetText(int index, string oldText)
        {
            if (oldText=="jamietre")
            {
                return "user:" + oldText;
            }
            else
            {
                return oldText;
            }
        }
   }
}