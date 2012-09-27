using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    public class CssStyle : ICSSStyle
    {
        public string Name { get; set; }
        public CSSStyleType Type { get; set; }
        public string Format { get; set; }
        public string Description { get; set; }
        public HashSet<string> Options { get; set; }

    }
}
