using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Server
{
    /// <summary>
    /// Data about a web request
    /// </summary>
    public interface ICsqWebRequestMetadata
    {
        int Timeout { get; set; }
        string UserAgent { get; set; }
    }
    public interface ICsqWebRequest : ICsqWebRequestMetadata
    {
        string Url { get; set; }
        CQ Dom { get; }
        bool Complete { get; }
    }
}
