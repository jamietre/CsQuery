using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Promises
{
    public delegate void PromiseAction<T>(T parameter);
    public delegate IPromise PromiseFunction<T>(T parameter);

    public interface IPromise
    {
        IPromise Then(Delegate success, Delegate failure=null);
        IPromise Then(Action success, Action failure = null);
        IPromise Then(Func<IPromise> success, Func<IPromise> failure = null);
        IPromise Then(PromiseAction<object> success, PromiseAction<object> failure = null);
        IPromise Then(PromiseFunction<object> success, PromiseFunction<object> failure = null);
    }

    public interface IPromise<T> : IPromise
    {

        IPromise Then(PromiseAction<T> success, PromiseAction<T> failure = null);
        IPromise Then(PromiseFunction<T> success, PromiseFunction<T> failure = null);
    }

}
