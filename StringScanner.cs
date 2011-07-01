using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{

    // Not implemented - intended to update the scanning code in Selector engine and maybe the HTML parser
    public class StringScanner
    {
        public StringScanner(string text)
        {
            Text = text;
        }
        protected string Text;
        protected int Pos;


    }
}
