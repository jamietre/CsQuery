using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CsQuery.Tests.Core.OutputFormatters_
{
    [TestFixture, TestClass]
    class FormatDefault
    {
        [Test, TestMethod]
        public void RenderLeadingXmlTag()
        {
            const string XmlTag = @"<?xml version=""1.0"" encoding=""UTF-8""?>";

            var formatter = new Output.FormatDefault();
            var comment = new CsQuery.Implementation.DomComment(XmlTag);
            var actual = formatter.Render(comment);

            Assert.AreEqual(XmlTag, actual, "FormatDefault should not change leading xml tag.");
        }
    }
}
