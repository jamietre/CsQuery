using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility
{
    /// <summary>
    /// Similar to framework Lazy&lt;T&gt; but provides a callback that can be used to inject dependencies in the lazily-created object.
    /// Damn you, you lazy object! Get a job!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyObject<T> where T : new()
    {
        protected Action<object> OnCreate = null;
        protected T _Object = default(T);
        public LazyObject()
        {

        }
        /// <summary>
        /// When a delegate onCreate is provided, it will be called with the new object instance after creation.
        /// </summary>
        /// <param name="onCreate"></param>
        public LazyObject(Action<object> onCreate)
        {
            OnCreate = onCreate;
        }
        
        public T Value
        {
            get
            {
                lock (this)
                {
                    if (_Object == null || _Object.Equals(default(T)))
                    {
                        _Object = new T();
                        if (OnCreate != null)
                        {
                            OnCreate(_Object);
                        }
                    }
                    _IsValueCreated = true;
                }
                return _Object;
            }
        }
        private bool _IsValueCreated = false;
        public bool IsValueCreated
        {
            get
            {
                return _IsValueCreated;
            }
        }
    }
}
