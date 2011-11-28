using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Server
{
    public enum RequestState
    {
        Idle=1,
        Active=2,
        Fail=3,
        PartialSuccess=4,
        Success=5
    }
    /// <summary>
    /// A group of async web requests. 
    /// </summary>
    /// 
    public interface IAsyncRequestQueue : ICsqWebRequest
    {
        void AddRequest(string url);
        void AddRequest(ICsqWebRequest request);
        IEnumerable<CsQuery> Results();
        RequestState State { get; }

    }
}
