using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    [Flags]
    public enum StartupOptions
    {

        /// <summary>
        /// When true, CsQuery will scan the client assembly for extensions. Any classes 
        /// found in a namespace CsQuery.Extensions will be configured automatically. Default is true; 
        /// disable this flag to disable this behavior
        /// </summary>

        LookForExtensions = 1
    }
}
