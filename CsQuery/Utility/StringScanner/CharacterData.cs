using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CsQuery.Utility.StringScanner;
using CsQuery.Utility.StringScanner.Implementation;

namespace CsQuery.Utility.StringScanner
{
    /// <summary>
    /// A static class to provide attribute information about characters, e.g. determining whether or not it
    /// belongs to a number of predefined classes. This creates an array of every possible character with a 
    /// ushort that is a bitmap (of up to 16 possible values, we can expand this to an int at some point if
    /// needed). This permits very fast access to this information since it only needs to be looked up
    /// via an index.
    /// 
    /// </summary>
    public static class CharacterData
    {
        private const string charsAlpha="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string charsNumeric = "0123456789";
        private const string charsNumber = "0123456789.-";
        private const string charsLower="abcdefghijklmnopqrstuvwxyz";
        private const string charsUpper="ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string charsWhitespace="\x0020\x0009\x000A\x000C\x000D\x00A0\x00C0";
        private const string charsQuote = "\"'";
        private const string charsOperator= "!+-*/%<>^=~";
        private const string charsEnclosing="()[]{}<>`´“”«»";
        private const string charsEscape="\\";
        private const string charsSeparators = ", |";


        private static ushort[] characterFlags;
        /// <summary>
        /// Configuration of the xref of character info
        /// </summary>
        static CharacterData()
        {
            characterFlags = new ushort[65536];
            setBit(charsWhitespace,(ushort)CharacterType.Whitespace);
            setBit(charsAlpha,(ushort)CharacterType.Alpha);
            setBit(charsNumeric,(ushort)CharacterType.Number);
            setBit(charsNumber,(ushort)CharacterType.NumberPart);
            setBit(charsLower,(ushort)CharacterType.Lower);
            setBit(charsUpper,(ushort)CharacterType.Upper);
            setBit(charsQuote,(ushort)CharacterType.Quote);
            setBit(charsOperator,(ushort)CharacterType.Operator);
            setBit(charsEnclosing,(ushort)CharacterType.Enclosing);
            setBit(charsEscape,(ushort)CharacterType.Escape);
            setBit(charsSeparators, (ushort)CharacterType.Separator);
            // html tag start
            
            SetHtmlTagNameStart((ushort)CharacterType.HtmlTagNameStart);
            SetHtmlTagNameChar();

            // html tag end
            setBit(" />",(ushort)CharacterType.HtmlTagEnd);
            // html tag any
            setBit("<>/", (ushort)CharacterType.HtmlTagAny);

            SetAlphaISO10646();
        }
        private static void SetAlphaISO10646()
        {
            ushort isoBit = (ushort)CharacterType.AlphaISO10646;
            setBit(charsAlpha, isoBit);
            setBit('-', isoBit);
            setBit('_', isoBit);
            // 161 = A1
            SetRange(isoBit,0x00A1,0xFFFF);
        }
        /// <summary>
        /// We omit ":" as a valid name start character because it makes pseudoselectors impossible to parse.
        /// </summary>
        /// <param name="hsb"></param>
        private static void SetHtmlTagNameStart(ushort hsb)
        {
            //  | [#xF900-#xFDCF] | [#xFDF0-#xFFFD] | [#x10000-#xEFFFF]

            setBit(charsAlpha, hsb);
            setBit("_", hsb);
            SetRange(hsb,0xC0,0xD6);
            SetRange(hsb, 0xD8, 0xF6);
            SetRange(hsb, 0xF8, 0x2FF);
            SetRange(hsb, 0x370, 0x37D);
            SetRange(hsb, 0x37F, 0x1FFF);
            SetRange(hsb, 0x200C, 0x200D);
            SetRange(hsb, 0x2070, 0x218F);
            SetRange(hsb, 0x2C00, 0x2FEF);
            SetRange(hsb, 0x3001, 0xD7FF);
            SetRange(hsb, 0xF900, 0xFDCF);
            SetRange(hsb, 0xFDF0, 0xFFFD);

            // what the heck is this? How can a unicode character be 32 bits?
            //SetRange(hsb, 0x10000, 0xEFFFF);
        }
        /// <summary>
        /// Similar to above, we omit "." as a valid in-name char because it breaks chained CSS selectors.
        /// </summary>
        private static void SetHtmlTagNameChar()
        {
            
            ushort hsb = (ushort)CharacterType.HtmlTagNameExceptStart;
            SetHtmlTagNameStart(hsb);
            setBit(charsNumeric, hsb);
            setBit("-", hsb);
            setBit((char)0xB7,hsb);
            SetRange(hsb, 0x0300, 0x036F);
            SetRange(hsb, 0x203F, 0x2040);
        }
        private static void SetRange(ushort flag, ushort start, ushort end)
        {
            for (int i = start; i <= end; i++)
            {
                setBit((char)i, flag);
            }
        }
        private static void setBit(string forCharacters, ushort bit)
        {
            for (int i=0;i<forCharacters.Length;i++) {
                setBit(forCharacters[i], bit);
            }
        }
        private static void setBit(char character, ushort bit)
        {
            characterFlags[(ushort)character] |= bit;
        }

        public static ICharacterInfo CreateCharacterInfo()
        {
            return new CharacterInfo();
        }
        public static ICharacterInfo CreateCharacterInfo(char character)
        {
            return new CharacterInfo(character);
        }
        public static IStringInfo CreateStringInfo()
        {
            return new StringInfo();
        }
        public static IStringInfo  CreateStringInfo(string text)
        {
            return new StringInfo(text);
        }
        public static bool IsType(char character, CharacterType type)
        {
            return (characterFlags[character] & (ushort)type) > 0;
        }
        public static CharacterType GetType(char character)
        {
            return (CharacterType)characterFlags[character];
        }
        /// <summary>
        /// Return the closing character for a set of known opening enclosing characters
        /// (including single and double quotes)
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static char Closer(char character)
        {
            char result = CloserImpl(character);
            if (result == (char)0)
            {
                throw new InvalidOperationException("The character '" + character + "' is not a known opening bound.");
            }
            return result;
        }
        private static char CloserImpl(char character) {
            switch (character)
            {
                case '"':
                    return '"';
                case '\'':
                    return '\'';
                case '[':
                    return ']';
                case '(':
                    return ')';
                case '{':
                    return '}';
                case '<':
                    return '>';
                case '`':
                    return '´';
                case '“':
                    return '”';
                case '«':
                    return '»';
                case '»':
                    return '«';
                default:
                   return (char)0;
            }
        }
        /// <summary>
        /// Return the matching bound for known opening and closing bound characters (same as Closer,
        /// but accepts closing tags and returns openers)
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static char MatchingBound(char character)
        {
            
            switch (character)
            {
                case ']':
                    return '[';
                case ')':
                    return '(';
                case '}':
                    return '{';
                case '>':
                    return '<';
                case '´':
                    return '`';
                case '”':
                    return '“';
                case '»':
                    return '«';
                default:
                    char result =  CloserImpl(character);
                    if (result == (char)0)
                    {
                        throw new InvalidOperationException("The character '" + character + "' is not a bound.");
                    };
                    return result;
            }
        }
    }
}
