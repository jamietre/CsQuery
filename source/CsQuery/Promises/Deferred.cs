using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CsQuery.Promises
{
    /// <summary>
    /// A Deferred object. Deferred objects implement the IPromise interface, and have methods for
    /// resolving or rejecting the promise.
    /// </summary>

    public class Deferred: IPromise
    {

        #region private properties

        private Func<object, IPromise> _Success;
        private Func<object, IPromise> _Failure;

        /// <summary>
        /// The success delegate
        /// </summary>

        protected Func<object, IPromise> Success
        {
            get
            {
                return _Success;
            }
            set
            {
                if (_Success != null)
                {
                    throw new InvalidOperationException("This promise has already been assigned a success delegate.");
                }
                _Success = value;
                if (Resolved == true)
                {
                    ResolveImpl();
                }
            }
        }

        /// <summary>
        /// The failure delegate
        /// </summary>

        protected Func<object, IPromise> Failure
        {
            get
            {
                return _Failure;
            }
            set
            {
                if (_Failure != null)
                {
                    throw new InvalidOperationException("This promise has already been assigned a failure delegate.");
                }
                _Failure = value;
                if (Resolved == false)
                {
                    RejectImpl();
                }
            }

        }

        /// <summary>
        /// The next deferred objected in the chain; resolved or rejected when any bound delegate is
        /// resolved or rejected./.
        /// </summary>

        protected Deferred NextDeferred;

        /// <summary>
        /// Indicates whether this object has been resolved yet. A null value means unresolved; true or
        /// false indicate success or failure.
        /// </summary>

        protected bool? Resolved;

        /// <summary>
        /// The parameter that was passed with a resolution or rejection.
        /// </summary>

        protected object Parameter;

        #endregion

        #region public methods

        /// <summary>
        /// Resolves the promise.
        /// </summary>
        ///
        /// <param name="parm">
        /// (optional) a value passed to the promise delegate
        /// </param>

        public void Resolve(object parm = null)
        {
            Parameter = parm;
            Resolved = true;
            ResolveImpl();
        }

        /// <summary>
        /// Rejects the promise
        /// </summary>
        ///
        /// <param name="parm">
        /// (optional) a value passed to the promise delegate.
        /// </param>

        public void Reject(object parm =null)
        {
            Parameter = parm;
            Resolved = false;
            RejectImpl();
        }

        /// <summary>
        /// Chains a delegate to be invoked upon resolution or failure of the Deferred promise object.
        /// </summary>
        ///
        /// <param name="success">
        /// The delegate to call upon successful resolution of the promise.
        /// </param>
        /// <param name="failure">
        /// (optional) The delegate to call upon unsuccessful resolution of the promise.
        /// </param>
        ///
        /// <returns>
        /// A new promise which will resolve when this promise has resolved.
        /// </returns>

        public IPromise Then(Delegate success, Delegate failure = null)
        {
            NextDeferred = new Deferred();

            MethodInfo method = success != null ?
                success.Method :
                failure.Method;

            Type returnType = method.ReturnType;
            Type[] parameters = method.GetParameters().Select(item => item.ParameterType).ToArray();

            bool useParms = parameters.Length > 0;

            Success = new Func<object, IPromise>((parm) =>
            {
                object result = success.DynamicInvoke(GetParameters(useParms));
                return result as IPromise;

            });
            Failure = new Func<object, IPromise>((parm) =>
            {
                object result = failure.DynamicInvoke(GetParameters(useParms));
                return result as IPromise;
            });

            return NextDeferred;
        }

        /// <summary>
        /// Chains delegates that will be executed on success or failure of a promise.
        /// </summary>
        ///
        /// <param name="success">
        /// The delegate to call upon successful resolution of the promise.
        /// </param>
        /// <param name="failure">
        /// (optional) The delegate to call upon unsuccessful resolution of the promise.
        /// </param>
        ///
        /// <returns>
        /// A new promise which will resolve when this promise has resolved.
        /// </returns>

        public IPromise Then(PromiseAction<object> success, PromiseAction<object> failure = null)
        {
            NextDeferred = new Deferred();

            Success = new Func<object, IPromise>((parm) =>
            {
                success(parm);
                return null;
            });
            if (failure != null)
            {
                Failure = new Func<object, IPromise>((parm) =>
                {
                    failure(parm);
                    return null;
                });

            }
            
            return NextDeferred;
        }

        /// <summary>
        /// Chains delegates that will be executed on success or failure of a promise.
        /// </summary>
        ///
        /// <param name="success">
        /// The delegate to call upon successful resolution of the promise.
        /// </param>
        /// <param name="failure">
        /// (optional) The delegate to call upon unsuccessful resolution of the promise.
        /// </param>
        ///
        /// <returns>
        /// A new promise which will resolve when this promise has resolved.
        /// </returns>

        public IPromise Then(PromiseFunction<object> success, PromiseFunction<object> failure = null)
        {
            NextDeferred = new Deferred();
            Success = new Func<object,IPromise>((parm) => {
                return success(Parameter);
            });
            if (failure != null)
            {
                Failure = new Func<object, IPromise>((parm) =>
                {
                    return success(Parameter);
                });
            }
            
            return NextDeferred;
        }

        /// <summary>
        /// Chains delegates that will be executed on success or failure of a promise.
        /// </summary>
        ///
        /// <param name="success">
        /// The delegate to call upon successful resolution of the promise.
        /// </param>
        /// <param name="failure">
        /// (optional) The delegate to call upon unsuccessful resolution of the promise.
        /// </param>
        ///
        /// <returns>
        /// A new promise which will be chained to this promise.
        /// </returns>

        public IPromise Then(Action success, Action failure = null)
        {
            NextDeferred = new Deferred();
            Success = new Func<object, IPromise>((parm) =>
            {
                success();
                return null;
            });
            if (failure != null)
            {
                Failure = new Func<object, IPromise>((parm) =>
                {
                    failure();
                    return null;
                });

            }
            
            return NextDeferred;
        }

        /// <summary>
        /// Chains delegates that will be executed on success or failure of a promise.
        /// </summary>
        ///
        /// <param name="success">
        /// The delegate to call upon successful resolution of the promise.
        /// </param>
        /// <param name="failure">
        /// (optional) The delegate to call upon unsuccessful resolution of the promise.
        /// </param>
        ///
        /// <returns>
        /// A new promise which will be chained to this promise.
        /// </returns>

        public IPromise Then(Func<IPromise> success, Func<IPromise> failure = null)
        {
            NextDeferred = new Deferred();
            Success = new Func<object, IPromise>((parm) =>
            {
                return success();
            });
            if (failure != null)
            {
                Failure = new Func<object, IPromise>((parm) =>
                {
                    return failure();
                });

            }
            
            return NextDeferred;
        }

        /// <summary>
        /// Gets the param,eter
        /// </summary>
        ///
        /// <param name="useParms">
        /// true to use parameters.
        /// </param>
        ///
        /// <returns>
        /// The parameters.
        /// </returns>

        protected object[] GetParameters(bool useParms)
        {
            object[] parms = null;

            if (useParms)
            {
                parms = new object[] { Parameter };
            }
            return parms;
        }

        /// <summary>
        /// Implementation of the Resolve function.
        /// </summary>

        protected void ResolveImpl()
        {
            if (Success != null)
            {
                try
                {
                    Success(Parameter);
                }
                catch
                {
                    RejectImpl();
                    return;
                }
            }
            if (NextDeferred != null)
            {
                NextDeferred.Resolve(Parameter);
            }
        }

        /// <summary>
        /// Implementation of the Reject function
        /// </summary>

        protected void RejectImpl()
        {
            if (Failure != null)
            {

                try
                {
                    Failure(Parameter);
                }
                catch { }
                
            }
            if (NextDeferred != null)
            {
                NextDeferred.Reject(Parameter);
            }
        }
        #endregion


    }
}
