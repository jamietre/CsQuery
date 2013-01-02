using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CsQuerySite
{
    public static class SiteConfig
    {
        public static bool IsDebug
        {
            get
            {
                return HttpContext.Current.IsDebuggingEnabled;
            }
        }
    }
}