using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Tests.jsdom
{
    public abstract class JsDomTest : CsQueryTest
    {
        protected abstract string Root { get; }
        protected IDomDocument Load(string file)
        {
            return TestDom("jsdom\\" + Root + "\\" + file + ".html").Document;
        }
    }

    
    public abstract class JsDomTest_Level2 : JsDomTest
    {

        protected override string Root
        {
            get { return "Level2"; }
        }
    }
}
