using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;

namespace CsQuery.Tests.Core.Selectors
{
    public partial class Css3: PseudoSelector
    {
        [Test, TestMethod]
        public void FirstChild()
        {
            var res = Dom["span :first-child"];
            Assert.AreEqual(4, res.Length);

            Assert.AreEqual("profile-triangle", res[0].ClassName);
            Assert.AreEqual("reputation-score", res[1].ClassName);
            Assert.AreEqual("badge2", res[2].ClassName);
            Assert.AreEqual("badge3", res[3].ClassName);
        }
    }
}