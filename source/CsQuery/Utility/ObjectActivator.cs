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
    /// Replacement for Activator.CreateInstance - see:
    /// http://mironabramson.com/blog/post/2008/08/Fast-version-of-the-ActivatorCreateInstance-method-using-IL.aspx
    /// </summary>

    public static class FastActivator
    {
      
        private static ConcurrentDictionary<Type,CreateObject> creatorCache = new ConcurrentDictionary<Type,CreateObject>();

        private readonly static Type coType = typeof(CreateObject);
        public delegate object CreateObject();

        /// <summary>
        /// Create an object that will used as a 'factory' to the specified type T 
        /// <returns></returns>
        public static object CreateInstance<T>() where T : class
        {
            return CreateInstance(typeof(T));
        }

        public static object CreateInstance(Type t)
        {
            CreateObject c;
            if (!creatorCache.TryGetValue(t, out c))
            {
                DynamicMethod dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + t.Name, t, null, t);
                ILGenerator ilGen = dynMethod.GetILGenerator();

                ilGen.Emit(OpCodes.Newobj, t.GetConstructor(Type.EmptyTypes));
                ilGen.Emit(OpCodes.Ret);
                c = (CreateObject)dynMethod.CreateDelegate(coType);
                creatorCache[t] = c;

            }
            return c();

        }

    }

}
