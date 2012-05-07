using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace CsQuery.Utility
{
    public class Browser
    {
        public Browser(HttpContext context)
        {
            Context = context;
            
        }
        public bool MSIE
        {
            get {
                return Context.Request.Browser.Browser == "IE";
            }
        }
        public bool Chrome
        {
            get
            {
                return Context.Request.Browser.Browser == "Chrome";
            }
        }
        public string Version {
            get {
                return Context.Request.Browser.Version;
            }
        }
        public int VersionMajor
        {
            get {
                return Context.Request.Browser.MajorVersion;
            }
        }
        private HttpContext Context;

    }
}
