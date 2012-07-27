using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Engine
{
    /// <summary>
    /// A case-insensitive character equality comparer.
    /// </summary>

    public class CharacterEqualityComparer: EqualityComparer<char>
    {
        public static CharacterEqualityComparer Create(bool isCaseSensitive) {
            return isCaseSensitive ? 
                new CaseSensitiveCharacterEqualityComparer():
                new CharacterEqualityComparer();
        }

        /// <summary>
        /// Tests if two char objects are considered equal.
        /// </summary>
        ///
        /// <param name="x">
        /// Character to be compared.
        /// </param>
        /// <param name="y">
        /// Character to be compared.
        /// </param>
        ///
        /// <returns>
        /// true if the objects are considered equal, false if they are not.
        /// </returns>

        public override bool Equals(char x, char y)
        {
            return x.Equals(y);
                
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

        public override int GetHashCode(char obj)
        {
            return obj.GetHashCode();
               
        }
    }

    /// <summary>
    /// A case-sensitive character equality comparer.
    /// </summary>

    public class CaseSensitiveCharacterEqualityComparer : CharacterEqualityComparer
    {
        /// <summary>
        /// Tests if two char objects are considered equal.
        /// </summary>
        ///
        /// <param name="x">
        /// Character to be compared.
        /// </param>
        /// <param name="y">
        /// Character to be compared.
        /// </param>
        ///
        /// <returns>
        /// true if the objects are considered equal, false if they are not.
        /// </returns>

        public override bool Equals(char x, char y)
        {
            return Char.ToLower(x).Equals(Char.ToLower(y));
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

        public override int  GetHashCode(char obj)
        {
            return Char.ToLower(obj).GetHashCode();
        }
    }
}
