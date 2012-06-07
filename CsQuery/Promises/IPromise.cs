using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Promises
{
    public interface IPromise
    {
        IPromise Then(Delegate success, Delegate failure=null);
        IPromise Then(Action success, Action failure = null);
        IPromise Then(Func<IPromise> success, Func<IPromise> failure = null);
        IPromise Then(Action<object> success, Action<object> failure = null);
        IPromise Then(Func<object, IPromise> success, Func<object, IPromise> failure = null);

    }
    public interface IPromise<T> : IPromise
    {
        IPromise Then(Action<T> success, Action<T> failure = null);
        IPromise Then(Func<T, IPromise> success, Func<T, IPromise> failure = null);
    }

}
