using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using CsQuery.Promises;

namespace CsQuery.Promises
{
    public class WhenAll: IPromise
    {
        public WhenAll(params IPromise[] promises)
        {
            Configure(promises);
        }

        public WhenAll(int timeoutMilliseconds, params IPromise[] promises)
        {
            Configure(promises);
            Timer = new Timer(timeoutMilliseconds);
            Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            Timer.Start();
        }
        
        protected Timer Timer;
        protected Deferred Deferred;
        protected int TotalCount = 0;
        private int _ResolvedCount = 0;
        protected int ResolvedCount
        {
            get
            {
                return _ResolvedCount;
            }

            set
            {
                _ResolvedCount = value;
                if (_ResolvedCount == TotalCount)
                {
                    if (Timer != null)
                    {
                        Timer.Stop();
                    }
                    if (Success)
                    {
                        Deferred.Resolve();
                    }
                    else
                    {
                        Deferred.Reject();
                    }
                }
            }
        }

        /// <summary>
        /// When false, means one or more of the promises was rejected, and the All will be rejected
        /// </summary>
        protected bool Success;

        protected void PromiseResolve()
        {
            ResolvedCount++;
        }
        protected void PromiseReject()
        {
            Success = false;
            ResolvedCount++;
        }
        public IPromise Then(Delegate success, Delegate failure = null)
        {
            return Deferred.Then(success,failure);
        }
        public IPromise Then(Action success, Action failure = null)
        {
            return Deferred.Then(success, failure);
        }
        public IPromise Then(Action<object> success, Action<object> failure = null)
        {
            return Deferred.Then(success, failure);
        }
        public IPromise Then(Func<IPromise> success, Func<IPromise> failure = null)
        {
            return Deferred.Then(success, failure);
        }
        public IPromise Then(Func<object, IPromise> success, Func<object, IPromise> failure = null)
        {
            return Deferred.Then(success, failure);
        }
        protected void Configure(IPromise[] promises)
        {
            Success = true;
            TotalCount = promises.Length;
            Deferred = new Deferred();
            foreach (var promise in promises)
            {
                promise.Then((Action)PromiseResolve,(Action)PromiseReject);
            }

        }
        protected void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Timer.Stop();
            Deferred.Reject();
        }
    }
}
