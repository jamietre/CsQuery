using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.StringScanner
{
    /// <summary>
    /// An interface for pattern matching. For each character, Expect will be called until it returns false.
    /// Validate will be called with the resulting string, and should return true or false to valide the entire pattern.
    /// Initialize allows setting up/capturing global data about the string in case other info is eneded
    /// </summary>
    public interface IExpectPattern
    {
        void Initialize(int startIndex, char[] source);
        /// <summary>
        /// Should return the next position (typically int++) or -1 if matching is complete
        /// </summary>
        /// <param name="index"></param>
        /// <param name="current"></param>
        /// <returns></returns>

        bool Validate();
        string Result { get; }
        int EndIndex { get; }
    }
}
