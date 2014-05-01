using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CsQuery.Tests.Core.Annotations
{
    [TestClass, TestFixture]
    public class Annotations
    {
        [TestMethod, Test]
        public void EmptyByDefault()
        {
            var cq = CQ.Create("<div></div>");

            int length = cq.FirstElement().Annotations.Length;
            Assert.AreEqual(0, length, "annotations is not empty");
        }

        [TestMethod, Test]
        public void GetMissingAnnotationReturnsNull()
        {
            var cq = CQ.Create("<div></div>");
            object value = cq.FirstElement().Annotations.GetAnnotation("missing");

            Assert.IsNull(value, "value was not null");
        }

        [TestMethod, Test]
        public void CanStoreAnnotations()
        {
            var cq = CQ.Create("<div></div>");
            var div = cq.FirstElement();

            div.Annotations.SetAnnotation("created-by", "User1");
            div.Annotations.SetAnnotation("times-created", 1);

            Assert.AreEqual(2, div.Annotations.Length, "annotation length is wrong");

            var createdBy = div.Annotations.GetAnnotation("created-by");
            var timesCreated = div.Annotations.GetAnnotation("times-created");

            Assert.AreEqual("User1", createdBy, "incorect created by value");
            Assert.AreEqual(1, timesCreated, "incorrect times created value");
        }

        [TestMethod, Test]
        public void CanRemoveAnnotations()
        {
            var cq = CQ.Create("<div></div>");
            var div = cq.FirstElement();

            div.Annotations.SetAnnotation("created-by", "User1");

            Assert.AreEqual(1, div.Annotations.Length, "initial annotations length is wrong");

            var createdBy = div.Annotations.GetAnnotation("created-by");
            Assert.AreEqual("User1", createdBy, "incorect created by value");

            div.Annotations.RemoveAnnotation("created-by");

            Assert.AreEqual(0, div.Annotations.Length, "final annotations length is wrong");
            Assert.IsNull(div.Annotations.GetAnnotation("created-by"));
        }

        [TestMethod, Test]
        public void NullOrWhiteSpaceAnnotationNamesNotPermitted()
        {
            var cq = CQ.Create("<div></div>");
            var a = cq.FirstElement().Annotations;

            Assert.Throws<ArgumentException>(() => a.GetAnnotation(null));
            Assert.Throws<ArgumentException>(() => a.GetAnnotation(""));
            Assert.Throws<ArgumentException>(() => a.GetAnnotation(" "));
            Assert.Throws<ArgumentException>(() => a.GetAnnotation("\t"));

            Assert.Throws<ArgumentException>(() => a.RemoveAnnotation(null));
            Assert.Throws<ArgumentException>(() => a.RemoveAnnotation(""));
            Assert.Throws<ArgumentException>(() => a.RemoveAnnotation(" "));
            Assert.Throws<ArgumentException>(() => a.RemoveAnnotation("\t"));

            Assert.Throws<ArgumentException>(() => a.SetAnnotation(null, "value"));
            Assert.Throws<ArgumentException>(() => a.SetAnnotation("", "value"));
            Assert.Throws<ArgumentException>(() => a.SetAnnotation(" ", "value"));
            Assert.Throws<ArgumentException>(() => a.SetAnnotation("\t", "value"));
        }

        [TestMethod, Test]
        public void CanUseIndexerToGetAndSetAnnotations()
        {
            var cq = CQ.Create("<div></div>");
            var a = cq.FirstElement().Annotations;

            Assert.IsNull(a["created-by"], "not initially null");

            a["created-by"] = "User1";

            Assert.AreEqual("User1", a["created-by"], "incorrect value");
        }
        
        [TestMethod, Test]
        public void CanEnumerateAnnotations()
        {
            var cq = CQ.Create("<div></div>");
            var a = cq.FirstElement().Annotations;

            Assert.AreEqual(0, a.ToArray().Length);

            a["created-by"] = "User1";
            a["times-created"] = 1;

            var annotations = a.ToArray();

            var correctKeys = annotations.Select(x => x.Key).SequenceEqual(new[] {"created-by", "times-created"});
            var correctValues = annotations.Select(x => x.Value).SequenceEqual(new object[] {"User1", 1});

            Assert.IsTrue(correctKeys, "incorrect keys");
            Assert.IsTrue(correctValues, "incorrect values");
        }
    }
}
