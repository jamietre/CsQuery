using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Tests
{
    /// <summary>
    /// Some simple utility functions to generate arrays
    /// </summary>
    public class Arrays: IEnumerable<string>
    {
        public static implicit operator Arrays(string members)
        {

            return new Arrays(members);

        }

        public Arrays(string members)
        {
            InnerList = new List<string>(members.SplitClean(','));

        }
        protected IEnumerable<string> InnerList;

        public static string[] String(params object[] members)
        {
            return Create<string>(members);

        }
        /// <summary>
        /// Create a new array from the parameters provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="members"></param>
        /// <returns></returns>
        public static T[] Create<T>(params object[] members) 
        {
            T[] arr = new T[members.Length];
            for (int i = 0; i < members.Length;i++ )
            {
                if (members[i] is T)
                {
                    arr[i] = (T)members[i];
                }
                else
                {

                    throw new ArgumentException("An argument was of the wrong type.");
                }


            }
            return arr;
        }
        /// <summary>
        /// Create a new array from the parameters provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="members"></param>
        /// <returns></returns>
        public static Array Create(params object[] members)
        {
            Type type = null;
            //T[] arr = new T[members.Length];
            for (int i = 0; i < members.Length; i++)
            {
                object item = members[i];
                if (type==null) {
                    type = item.GetType();
                } else {
                    if (type != item.GetType()) {
                        type = typeof(object);
                        break;
                    }
                }
            }

            Array arr  = Array.CreateInstance(type, members.Length);

             members.CopyTo(arr,0);
            return arr;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
