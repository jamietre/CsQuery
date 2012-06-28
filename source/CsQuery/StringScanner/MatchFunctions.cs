using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.StringScanner
{
    public static class MatchFunctions
    {
        public static bool Alpha(int index, char character)
        {
            return CharacterData.IsType(character, CharacterType.Alpha); 
        }
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
        public static bool PseudoSelector(int index, char character)
        {
            return index == 0 ? CharacterData.IsType(character, CharacterType.Alpha) :
               CharacterData.IsType(character, CharacterType.Alpha) || character == '-';
        }
        /// <summary>
        /// Matches a valid CSS class: http://www.w3.org/TR/CSS21/syndata.html#characters
        /// Does not currently deal with escaping though.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="character"></param>
        /// <returns></returns>
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
