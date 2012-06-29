using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using CsQuery;

namespace CsQuery.Web
{
    public interface ICsqWebResponse: ICsqWebRequest
    {
        string Html { get; }
        DateTime? Started { get; }
        DateTime? Finished { get; }
        bool Success { get; }
        WebException WebException { get; }
    }
}
