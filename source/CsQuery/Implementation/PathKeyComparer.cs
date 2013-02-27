using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public unsafe  int Compare3(ushort[] x, ushort[] y)
        {
            int pos = 0;
            int len = x.Length < y.Length ? x.Length : y.Length;
            fixed (ushort* fpx = &x[0], fpy = &y[0])
            {
                ushort* px = fpx;
                ushort* py = fpy;
                while (pos < len && *px == *py)
                {
                    px++;
                    py++;
                    pos++;
                }
            }

            return pos < len ?
                 (x[pos] < y[pos] ? -1 : 1) :
                 (x.Length < y.Length ? -1 : x.Length > y.Length ? 1 : 0);
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
            int xlen = x.Length;
            int ylen = y.Length;

            int ilen = xlen < ylen ? xlen :  ylen;
            int ipos = 0;

            while (ipos < ilen && x[ipos] == y[ipos])
                ++ipos;

            return ipos < ilen
                        ? (x[ipos] < y[ipos] ? -1 : 1)
                        : (xlen <ylen ? -1 : xlen > ylen ? 1 : 0);
        }






            //int pos = 0;
            //int len = Math.Min(x.Length, y.Length);

            //// the below is probably not worth it for less than 5 (or so) elements,
            ////   so just do the old way
            //if (len < 5)
            //{
            //  while (pos < len && x[pos] == y[pos])
            //    ++pos;

            //  return pos < len ?
            //    x[pos].CompareTo(y[pos]) :
            //    x.Length.CompareTo(y.Length);
            //}

            //ushort lastX = x[len-1];
            //bool lastSame = true;

            //if (x[len-1] == y[len-1])
            //    --x[len-1]; // can be anything else
            //else
            //    lastSame = false;

            //while (x[pos] == y[pos])
            //    ++pos;


            //return pos < len-1 ?
            //    x[pos].CompareTo(y[pos]) :
            //    lastSame ? x.Length.CompareTo(y.Length)
            //             : lastX.CompareTo(y[len-1]);

        
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(ushort[] b1, ushort[] b2, long count);

        public int Compare4(ushort[] x, ushort[] y)
        {
            int xLen = x.Length, yLen = y.Length;

            int len = xLen < yLen
                          ? xLen
                          : yLen;


            var samePartEqual = memcmp(x, y, len * 2) == 0;

            if (samePartEqual)
            {
                if (xLen == yLen)
                {
                    return 0;
                }
                return xLen < yLen
                       ? -1
                       : xLen > yLen ?
                        1 : 0;
            }


            int pos = 0;
            while (x[pos] == y[pos])
                pos++;

            return (x[pos] < y[pos]
                        ? -1
                        : x[pos] > y[pos] ? 1 : 0);
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
