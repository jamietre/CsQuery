using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Promises
{
    //public class Deferred<T>: IPromise<T>
    //{
    //    #region private properties

    //    private Func<T, IPromise> _Success;
    //    protected Func<T, IPromise> Success
    //    {
    //        get
    //        {
    //            return _Success;
    //        }
    //        set
    //        {
    //            _Success = value;
    //            if (Resolved==true)
    //            {
    //                _Success(Parameter);
    //            }
    //        }
    //    }
    //    private Func<T, IPromise> _Failure;
    //    protected Func<T, IPromise> Failure
    //    {
    //        get
    //        {
    //            return _Failure;
    //        }
    //        set
    //        {
    //            _Failure = value;
    //            if (Resolved == false)
    //            {
    //                _Failure(Parameter);
    //            }
    //        }

    //    }

    //    protected bool? Resolved;

    //    protected T Parameter;

    //    #endregion

    //    #region public methods

    //    public void Resolve(T parm=default(T))
    //    {
    //        Parameter = parm;
    //        Resolved=true;
    //        if (Success != null)
    //        {
    //            Success(Parameter);
    //        }

    //    }
    //    public void Reject(T parm=default(T))
    //    {
    //        Parameter = parm;
    //        Resolved = false;
    //        if (Success != null)
    //        {
    //            Success(Parameter);

    //        }

    //    }

    //    public IPromise Then(Action<T> success, Action<T> failure = null)
    //    {
    //        return Then<T>(success, failure);
    //    }
    //    public IPromise Then<U>(Action<U> success, Action<U> failure=null)
    //    {
    //        Success = new Func<T,IPromise>((parm) =>
    //        {
    //            success(parm);
    //            return null;
    //        });
    //        if (failure != null)
    //        {
    //            Failure = new Func<T, IPromise>((parm) =>
    //            {
    //                failure(parm);
    //                return null;
    //            });

    //        }
    //        return new Deferred<T>();
    //    }
    //    public IPromise Then(Func<T, IPromise> success, Func<T, IPromise> failure = null)
    //    {
    //        return Then<T>(success, failure);
    //    }
    //    public IPromise Then<U>(Func<T,IPromise> success, Func<T,IPromise> failure = null)
    //    {
    //        Success = success;
    //        Failure = failure;
    //        return new Deferred<U>();
    //    }
    //    public IPromise Then(Action success, Action failure = null)
    //    {
    //        Success = new Func<T, IPromise>((parm) =>
    //        {
    //            success();
    //            return null;
    //        });
    //        if (failure != null)
    //        {
    //            Failure = new Func<T, IPromise>((parm) =>
    //            {
    //                failure();
    //                return null;
    //            });

    //        }
    //        return new Deferred<T>();
    //    }
    //    public IPromise Then(Func<IPromise> success, Func<IPromise> failure = null)
    //    {
    //        Success = new Func<T, IPromise>((parm) =>
    //        {
    //            return success();
    //        });
    //        if (failure != null)
    //        {
    //            Failure = new Func<T, IPromise>((parm) =>
    //            {
    //                return failure();
    //            });

    //        }
    //        return new Deferred<T>();
    //    }
    //    public IPromise Then(Delegate success, Delegate failure=null)
    //    {
    //        Type returnType = success.Method.ReturnType;
    //        Type[] parameters = success.Method.GetParameters().Select(item=>item.ParameterType).ToArray();

    //        object[] parms=null;

    //        if (parameters.Length > 0)
    //        {
    //            parms = new object[] { Parameter };
    //        }

    //        Success = new Func<T, IPromise>((parm) =>
    //        {
    //            object result = success.DynamicInvoke(parms);
    //            return result as IPromise;

    //        });
    //        Failure = new Func<T, IPromise>((parm) =>
    //        {
    //            object result = failure.DynamicInvoke(parms);
    //            return result as IPromise;
    //        });
    //        return new Deferred<T>();
    //    }

    //    #endregion
    //}
}
