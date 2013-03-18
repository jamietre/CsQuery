using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.EquationParser;
using CsQuery.EquationParser.Implementation;
using CsQuery.EquationParser.Implementation.Functions;
using CsQuery.Implementation;
using CsQuery.Engine;

namespace CsQuery.Tests.Core.Dom
{
    [TestFixture, TestClass]
    public class RangedIndexTests
    {
        [Test, TestMethod]
        public void DomIndexRangedTests()
        {
            var indexTests = new SharedIndexTests<DomIndexRanged>();
            indexTests.RunAllTests();
        }

        [Test, TestMethod]
        public void DomIndexSimpleTests()
        {
            var indexTests = new SharedIndexTests<DomIndexSimple>();
            indexTests.RunAllTests();
        }
    }
}

