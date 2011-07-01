using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


/*
 * Plugin for parsing server posted data. Just add this namespace.
 */
namespace Jtc.CsQuery.Server
{
    public static class ExtensionMethods
    {
        public static CsQueryHttpContext Server(this CsQuery owner) {
            object extInstance;
            if (owner.ExtensionCache.TryGetValue("Server", out extInstance))
            {
                return (CsQueryHttpContext)extInstance;
            }
            else
            {
                CsQueryHttpContext newInstance = new CsQueryHttpContext(owner, HttpContext.Current);
                owner.ExtensionCache.Add("Server", newInstance);
                return newInstance;
            }
           
        }
    }

}
