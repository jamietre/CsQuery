﻿using System;
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
    public class RangedIndexTests: SharedIndexTests<DomIndexRanged>
    {
        [Test, TestMethod]
        public void RunTests()
        {
            // not yet ready..
            RunAllTests();
        }
    }
}

