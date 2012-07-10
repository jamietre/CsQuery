using System;
using System.Collections.Generic;
using System.Web;
using CsQuery;

namespace CsQuery.AspNet
{
    public interface ICsQueryControl
    {
        CQ Doc { get; }
    }
}