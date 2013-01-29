using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsQuery.Implementation
{
    /// <summary>
    /// A string comparer that is not concerned with anything other than the raw value of the characters. No encoding, no culture.
    /// </summary>

    public class PathKeyComparer : IComparer<ushort[]>, IEqualityComparer<ushort[]>
    {
        static PathKeyComparer()
        {
            _Comparer = new PathKeyComparer();
        }


        private static PathKeyComparer _Comparer;

        /// <summary>
        /// Gets an instance of TrueStringComparer
        /// </summary>

        public static PathKeyComparer Comparer
        {
            get
            {
                return _Comparer;
            }
        }

        /// <summary>
        /// Compares two string objects to determine their relative ordering.
        /// </summary>
        ///
        /// <param name="x">
        /// String to be compared.
        /// </param>
        /// <param name="y">
        /// String to be compared.
        /// </param>
        ///
        /// <returns>
        /// Negative if 'x' is less than 'y', 0 if they are equal, or positive if it is greater.
        /// </returns>

        public int Compare(ushort[] x, ushort[] y)
        {
            int pos = 0;
            int len = Math.Min(x.Length, y.Length);
            while (pos < len && x[pos] == y[pos])
                pos++;

            return pos<len ?
                x[pos].CompareTo(y[pos]) :
                x.Length.CompareTo(y.Length);

        }

        /// <summary>
        /// Marginally faster when just testing equality than using Compare
        /// </summary>
        ///
        /// <param name="x">
        /// String to be compared.
        /// </param>
        /// <param name="y">
        /// String to be compared.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        protected bool CompareEqualLength(ushort[] x, ushort[] y)
        {
            int len = x.Length;
            for (int pos = 0; pos < len; pos++)
            {
                if (x[pos] != y[pos])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tests if two string objects are considered equal.
        /// </summary>
        ///
        /// <param name="x">
        /// String to be compared.
        /// </param>
        /// <param name="y">
        /// String to be compared.
        /// </param>
        ///
        /// <returns>
        /// true if the objects are considered equal, false if they are not.
        /// </returns>

        public bool Equals(ushort[] x, ushort[] y)
        {
            return x.Length == y.Length && CompareEqualLength(x, y);
        }

        /// <summary>
        /// Calculates the hash code for this object.
        /// </summary>
        ///
        /// <param name="obj">
        /// The object.
        /// </param>
        ///
        /// <returns>
        /// The hash code for this object.
        /// </returns>
        
        

        public int GetHashCode(ushort[] obj)
        {
            unchecked
            {
                        
                const int HashP = 16777619;
                int hash = (int)2166136261;

                for (ushort i = 0; i < obj.Length; i++)
                    hash = (hash ^ obj[i]) * HashP;
                

                //hash += hash << 13;
                //hash ^= hash >> 7;
                //hash += hash << 3;
                //hash ^= hash >> 17;
                //hash += hash << 5;
                
                return ((((hash + (hash << 13))
                    ^ (hash >> 7))
                    + (hash << 3))
                    ^ (hash >> 17))
                    + (hash << 5);
            }
            
        }

    }
}
