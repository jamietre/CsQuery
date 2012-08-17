using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using CsQuery.Promises;

namespace CsQuery.Promises
{
    /// <summary>
    /// A promise that resolves when one or more other promises have all resolved
    /// </summary>

    public class WhenAll: IPromise
    {
        /// <summary>
        /// Constructor
        /// </summary>
        ///
        /// <param name="promises">
        /// A variable-length parameters list containing promises that must all resolve
        /// </param>

        public WhenAll(params IPromise[] promises)
        {
            Configure(promises);
        }

        /// <summary>
        /// Constructor to create a promise that resolves when one or more other promises have all
        /// resolved or a timeout elapses.
        /// </summary>
        ///
        /// <param name="timeoutMilliseconds">
        /// The timeout in milliseconds.
        /// </param>
        /// <param name="promises">
        /// A variable-length parameters list containing promises that must all resolve.
        /// </param>

        public WhenAll(int timeoutMilliseconds, params IPromise[] promises)
        {
            Configure(promises);
            Timer = new Timer(timeoutMilliseconds);
            Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            Timer.Start();
        }
        
        private Timer Timer;
        private  Deferred Deferred;
        private int TotalCount = 0;
        private int _ResolvedCount = 0;
        private int ResolvedCount
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
        /// When false, means one or more of the promises was rejected, and the All will be rejected.
        /// </summary>

        private  bool Success;

        private void PromiseResolve()
        {
            ResolvedCount++;
        }
        private void PromiseReject()
        {
            Success = false;
            ResolvedCount++;
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

        public IPromise Then(Delegate success, Delegate failure = null)
        {
            return Deferred.Then(success,failure);
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
            return Deferred.Then(success, failure);
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
            return Deferred.Then(success, failure);
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
            return Deferred.Then(success, failure);
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

        public IPromise Then(PromiseFunction<object> success,PromiseFunction<object> failure = null)
        {
            return Deferred.Then(success, failure);
        }

        private void Configure(IPromise[] promises)
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
