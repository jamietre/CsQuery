using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Output;

namespace CsQuery
{
    public static class HtmlEncoders
    {
        public static IHtmlEncoder Default = new HtmlEncoderDefault();
        public static IHtmlEncoder Minimum = new HtmlEncoderMinimum();
        public static IHtmlEncoder None = new HtmlEncoderNone();

    }
}
