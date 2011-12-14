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
    public class LazyObjectFrom<T>
    {
        protected Action<object> OnCreate = null;
        protected Func<T> GetNewObject = null;

        protected T _Object = default(T);

        /// <summary>
        /// The object will be obtained from getNewObject delegate
        /// </summary>
        /// <param name="getNewObject"></param>
        public LazyObjectFrom(Func<T> getNewObject)
        {
            GetNewObject = getNewObject;
        }
        public LazyObjectFrom(Func<T> getNewObject,Action<object> onCreate)
        {
            OnCreate = onCreate;
            GetNewObject = getNewObject;
        }

        public T Value
        {
            get
            {
                lock (this)
                {
                    if (_Object == null || _Object.Equals(default(T)))
                    {
                        _Object = GetNewObject();
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


    public class LazyObject<T>: LazyObjectFrom<T> where T: new()
    {
        public LazyObject(): base(GetNewObject)
        {

        }
        /// <summary>
        /// When a delegate onCreate is provided, it will be called with the new object instance after creation.
        /// </summary>
        /// <param name="onCreate"></param>
        public LazyObject(Action<object> onCreate): base(GetNewObject,onCreate)
        {
            OnCreate = onCreate;
        }

        protected static T GetNewObject() {
            return new T();
        }
    }

}
