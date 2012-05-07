using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    [Flags]
    public enum DomRenderingOptions
    {
        RemoveMismatchedCloseTags = 1,
        RemoveComments = 2,
        QuoteAllAttributes = 4,
        ValidateCss = 8
    }
}
