using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using CsQuery;
using CsQuery.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using MsAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Diagnostics;
using CsQuery.EquationParser;

namespace CsQuery.Tests.Performance
{
    [TestClass]
    public class _Performance_MediumDom : PerformanceShared
    {
        protected override string DocName
        {
            get
            {
                return "wiki-cheese";
            }
        }
                protected override string DocDescription
        {
            get { return "medium document (wiki-cheese, about 170k)"; }
        }
    }
}
