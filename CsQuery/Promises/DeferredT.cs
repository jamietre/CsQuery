using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Promises
{
    public class Deferred<T> : Deferred, IPromise<T>
    {

        public IPromise Then(Action<T> success, Action<T> failure = null)
        {
            return base.Then(success, failure);
        }

        public IPromise Then(Func<T, IPromise> success, Func<T, IPromise> failure = null)
        {
            return base.Then(success, failure);
        }
    }
   
}
