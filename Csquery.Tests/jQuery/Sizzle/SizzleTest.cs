using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;

namespace CsqueryTests.jQuery.Sizzle
{
    public class SizzleTest: CsQueryTest
    {
        public override void  FixtureSetUp()
        {
 	         base.FixtureSetUp();
             Dom = TestDom("sizzle");
        }

        protected CQ Sizzle
        {
            get
            {
                return Dom;
            }
        }
    }
}
