using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace CsQuery.Utility
{
    /// <summary>
    /// Replacement for Activator.CreateInstance that caches the constructor fucnction, providing a
    /// significant performance improvement over Activator.CreateInstance. Calling with a value type
    /// will be deferred to Activator.CreateInstance.
    /// </summary>
    /// 
    ///
    /// <url>
    /// http://mironabramson.com/blog/post/2008/08/Fast-version-of-the-ActivatorCreateInstance-method-using-IL.aspx
    /// </url>

    public static class FastActivator
    {
      
        private static ConcurrentDictionary<Type,CreateObject> creatorCache = new ConcurrentDictionary<Type,CreateObject>();

        private readonly static Type coType = typeof(CreateObject);
        private delegate object CreateObject();

        /// <summary>
        /// Create a new instance of type T
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The type of object to create
        /// </typeparam>
        ///
        /// <returns>
        /// A new instance of type T
        /// </returns>

        public static T CreateInstance<T>() where T : class
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        ///
        /// <param name="type">
        /// The Type to process.
        /// </param>
        ///
        /// <returns>
        /// The new instance.
        /// </returns>

        public static object CreateInstance(Type type)
        {
            CreateObject c;
            if (!type.IsClass)
            {
                return Activator.CreateInstance(type);
                
            }

            if (!creatorCache.TryGetValue(type, out c))
            {
                DynamicMethod dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + type.Name, type, null, type);
                ILGenerator ilGen = dynMethod.GetILGenerator();

                ilGen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
                ilGen.Emit(OpCodes.Ret);
                c = (CreateObject)dynMethod.CreateDelegate(coType);
                creatorCache[type] = c;

            }
            return c();

        }

    }

}
