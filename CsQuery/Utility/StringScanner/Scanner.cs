using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.StringScanner
{
    public static class Scanner
    {
        public static IStringScanner Create(string Text)
        {
            return new Implementation.StringScanner(Text);
        }
    }
}
