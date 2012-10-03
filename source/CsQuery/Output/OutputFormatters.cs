using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Output
{
    public static class OutputFormatters
    {
        public IOutputFormatter Default(IHtmlEncoder encoder = null)
        {
            encoder = encoder ?? new HtmlEncoderDefault();
            return new OutputFormatterDefault(encoder);
        }
    }
}
