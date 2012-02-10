using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


/*
 * Plugin for parsing server posted data. Just add this namespace.
 */
namespace CsQuery.Server
{
    public static class ExtensionMethods
    {
        public static CsQueryHttpContext Server(this CQ owner) {
            return new CsQueryHttpContext(owner,HttpContext.Current);
        }
    }

}
