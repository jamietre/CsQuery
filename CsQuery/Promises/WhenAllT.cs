using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using CsQuery.Promises;

namespace CsQuery.Promises
{
    public class WhenAll : WhenAll<object>
    {
        public WhenAll(params IPromise[] promises): base(promises)
        {
          
        }

        public WhenAll(int timeoutMilliseconds, params IPromise[] promises):
            base(timeoutMilliseconds,promises)
        {
        
        }
    }
}
