using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.StringScanner.Implementation;

namespace CsQuery.StringScanner.Patterns
{
    /// <summary>
    /// ID and NAME tokens must begin with a letter ([A-Za-z]) and may be followed by any number of letters, 
    /// digits ([0-9]), hyphens ("-"), underscores ("_"), colons (":"), and periods (".").
    /// </summary>
    public class HtmlID: EscapedString
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public HtmlID(): 
            base(IsValidID)
        {

        }

        /// <summary>
        /// Match a pattern for a valid HTML ID.
        /// </summary>
        ///
        /// <param name="index">
        /// .
        /// </param>
        /// <param name="character">
        /// .
        /// </param>
        ///
        /// <returns>
        /// true if valid identifier, false if not.
        /// </returns>

        protected static bool IsValidID(int index, char character)
        {

            if (index == 0)
            {
                return CharacterData.IsType(character, CharacterType.AlphaISO10646);
            }
            else
            {
                return CharacterData.IsType(character, CharacterType.AlphaISO10646 | CharacterType.Number);
            }
        }
    }
}
