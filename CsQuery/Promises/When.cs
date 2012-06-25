using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Promises;

namespace CsQuery
{
    public static class When
    {
        public static Deferred Deferred()
        {
            return new Deferred();
        }
        public static Deferred<T> Deferred<T>()
        {
            return new Deferred<T>();
        }

        public static IPromise All(params IPromise[] promises)
        {
            return new WhenAll(promises);

        }
        public static IPromise All(int timeoutMilliseconds, params IPromise[] promises)
        {
            return new WhenAll(timeoutMilliseconds,promises);
            
        }
    }
}
