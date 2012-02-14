using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;

namespace CsQuery.Web
{
    public interface ICsqWebResponse: ICsqWebRequest
    {
        string Html { get; }
        CQ Dom { get; } 
        DateTime Started { get; }
        DateTime Finished { get; }
        bool Complete { get; }
    }
}
