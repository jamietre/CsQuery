using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CsQuery.Promises
{
    public class Deferred: IPromise
    {

        #region private properties

        private Func<object, IPromise> _Success;
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
        private Func<object, IPromise> _Failure;
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

        protected Deferred NextDeferred;
        protected bool? Resolved;

        protected object Parameter;

        #endregion

        #region public methods

        public void Resolve(object parm = null)
        {
            Parameter = parm;
            Resolved = true;
            ResolveImpl();
        }
       
        public void Reject(object parm =null)
        {
            Parameter = parm;
            Resolved = false;
            RejectImpl();
        }

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
        
        protected object[] GetParameters(bool useParms)
        {
            object[] parms = null;

            if (useParms)
            {
                parms = new object[] { Parameter };
            }
            return parms;
        }

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
