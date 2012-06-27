﻿using System;
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
    public class PerfData
    {
        public string Source { get; set; }
        public int Iterations { get; set; }
        public TimeSpan Time { get; set; }
        public string ErrorMessage { get; set; }

        public double TimeSeconds
        {
            get
            {
                return Math.Round(Time.TotalSeconds, 1);
            }
        }

        public double IterationsPerSecond {
            get {
                return (double)Iterations / ((double)Time.TotalMilliseconds / 1000d);
            }
        }
        

    }
}
