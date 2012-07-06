using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.StringScanner
{
    /// <summary>
    /// Match functions. These are used with StringScanner to parse out expected strings. A basic
    /// match function accepts an int and a char, and is eand returns true as long as the character
    /// is valid for that position in the string. Many patterns have different valid first characters
    /// versus later characters. The function will be called beginning with index zero, and continue
    /// to be called until it returns false, indicating that the end of a pattern that matches that
    /// concept has been reached.
    /// 
    /// More complex patterns require a memory of the previous state, for example, to know whether
    /// quoting is in effect. the IExpectPattern interface describes a class to match more complex
    /// patterns.
    /// </summary>

    public static class MatchFunctions
    {
        /// <summary>
        /// Return true while the string is alphabetic, e.g. contains only letters.
        /// </summary>
        ///
        /// <param name="index">
        /// Zero-based index of the current position in the string.
        /// </param>
        /// <param name="character">
        /// The character at the current position.
        /// </param>
        ///
        /// <returns>
        /// True if the current character is valid for this pattern, false if not.
        /// </returns>

        public static bool Alpha(int index, char character)
        {
            return CharacterData.IsType(character, CharacterType.Alpha); 
        }

        /// <summary>
        /// Returns a pattern that matches numbers.
        /// </summary>
        ///
        /// <param name="requireWhitespaceTerminator">
        /// (optional) when true, only whitespace can terminate this number. When false, any non-numeric character will succesfully terminate the pattern.
        /// </param>
        ///
        /// <returns>
        /// The total number of ber.
        /// </returns>

        public static IExpectPattern Number(bool requireWhitespaceTerminator = false)
        {
            var pattern = new Patterns.Number();
            pattern.RequireWhitespaceTerminator = requireWhitespaceTerminator;
            return pattern;
        }


        public static bool Alphanumeric(int index, char character)
        {
            return CharacterData.IsType(character, CharacterType.Alpha | CharacterType.NumberPart);
        }
        public static IExpectPattern HtmlIDValue()
        {
            // The requirements are different for HTML5 vs. older HTML specs but basically we don't want to be
            // too rigorous on this one -- the tagname spec is about right and includes underscores & colons

            return new Patterns.HtmlID();
        }
        public static IExpectPattern HTMLAttribute()
        {
            return new Patterns.HTMLAttributeName();
          
        }
        public static IExpectPattern HTMLTagSelectorName()
        {
            return new Patterns.HTMLTagSelectorName();
        }

        public static IExpectPattern BoundedBy(string boundStart=null, string boundEnd=null, bool honorInnerQuotes=false)
        {
            
            var pattern = new Patterns.Bounded();
            if (!String.IsNullOrEmpty(boundStart))
            {
                pattern.BoundStart = boundStart;
            }
            if (!String.IsNullOrEmpty(boundEnd))
            {
                pattern.BoundEnd= boundEnd;
            }
            pattern.HonorInnerQuotes = honorInnerQuotes;
            return pattern;
        }

        public static IExpectPattern Bounded
        {
            get
            {
                var pattern = new Patterns.Bounded();
                pattern.HonorInnerQuotes = false;
                return pattern;
            }
        }
        public static IExpectPattern BoundedWithQuotedContent
        {
            get
            {
                var pattern = new Patterns.Bounded();
                pattern.HonorInnerQuotes = true;
                return pattern;
            }
        }
        public static bool NonWhitespace(int index, char character)
        {
            return !CharacterData.IsType(character, CharacterType.Whitespace); 
        }
        public static bool QuoteChar(int index, char character)
        {
            return CharacterData.IsType(character, CharacterType.Quote); 
        }
        public static bool QuoteChar(int index, char character)
        {
            return CharacterData.IsType(character, CharacterType.Quote);
        }
        public static bool BoundChar(int index, char character)
        {
            return CharacterData.IsType(character, CharacterType.Enclosing | CharacterType.Quote); 
        }

        public static IExpectPattern Quoted
        {
            get
            {
                return new Patterns.Quoted();
            }
        }

        /// <summary>
        /// A matching function that validates 
        /// </summary>
        ///
        /// <param name="index">
        /// Zero-based index of the.
        /// </param>
        /// <param name="character">
        /// The character.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        public static bool PseudoSelector(int index, char character)
        {
            return index == 0 ? CharacterData.IsType(character, CharacterType.Alpha) :
               CharacterData.IsType(character, CharacterType.Alpha) || character == '-';
        }

        /// <summary>
        /// Matches a valid CSS class: http://www.w3.org/TR/CSS21/syndata.html#characters Does not
        /// currently deal with escaping though.
        /// </summary>
        ///
        /// <value>
        /// The name of the CSS class.
        /// </value>

        public static IExpectPattern CssClassName
        {
            get {
                return new Patterns.CssClassName();
            }
        }
        public static IExpectPattern OptionallyQuoted
        {
            get
            {
                return new Patterns.OptionallyQuoted();
            }
        }
        public static bool Operator(int index, char character)
        {
            return CharacterData.IsType(character, CharacterType.Operator);
        }

    }

}
