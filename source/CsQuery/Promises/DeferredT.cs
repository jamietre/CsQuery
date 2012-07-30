using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Promises
{
    public class Deferred<T> : Deferred, IPromise<T>
    {

        public IPromise Then(PromiseAction<T> success, PromiseAction<T> failure = null)
        {
            return base.Then(success, failure);
        }

        public IPromise Then(PromiseFunction<T> success, PromiseFunction<T> failure = null)
        {
            return base.Then(success, failure);
        }

     
    }
   
}
