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

    public class TrueStringComparer : IComparer<string>, IEqualityComparer<string>
    {
        static TrueStringComparer() {
            _Comparer = new TrueStringComparer();
        }


        private static TrueStringComparer _Comparer;

        /// <summary>
        /// Gets an instance of TrueStringComparer
        /// </summary>

        public static TrueStringComparer Comparer
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

        public int Compare(string x, string y)
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

        protected bool CompareEqualLength(string x, string y)
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

        public bool Equals(string x, string y)
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

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
