using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsqueryTests
{
    public static class ExtensionMethods
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> baseList)
        {
            if (baseList == null) return true;
            return baseList.IsEmpty();
        }
        public static bool IsEmpty<T>(this IEnumerable<T> baseList)
        {
            if (baseList == null) return false;
            bool result = true;
            // I think this is the most efficient way to verify an empty IEnumerable
            foreach (T t in baseList)
            {
                result = false;
                break;
            }
            return (result);
        }
    }
}
