using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CsQuery.Tests.Csharp
{
    public partial class Methods : CsQueryTest
    {
        [Test, TestMethod]
        public void PrependBug()
        {
            var result = CQ.CreateFragment("<b>1</b><b>2</b><b>3</b>").Select("b").Prepend("__").Append("__").Text();
            Assert.AreEqual("__1____2____3__", result);
        }
    }
}
