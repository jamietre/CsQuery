using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    /// <summary>
    /// A single CSS style
    /// </summary>
    public interface ICssStyle
    {
        string Name { get; set; }
        CssStyleType Type { get; set; }
        string Format { get; set; }
        HashSet<string> Options { get; set; }
        string Description { get; set; }

    }
}
