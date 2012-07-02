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

namespace CsQuery.Tests._Performance
{
    [TestClass]
    public class _Performance_BigDom : PerformanceShared
    {
        protected override string DocName
        {
            get
            {
                return "HTML standard";
            }
        }

        protected override string DocDescription
        {
            get { return "large document (HTML Standard, about 6,000k)"; }
        }

        /// <summary>
        /// For the large test, we need more time since it can take 2 seconds just to load the DOM
        /// </summary>
        protected override int TestTimeSeconds
        {
            get
            {
                return 10;
            }
        }
    }
}
