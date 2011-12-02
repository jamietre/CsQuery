using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery
{

    // Not implemented - intended to update the scanning code in Selector engine and maybe the HTML parser
    public class StringScanner
    {
        public StringScanner()
        {
            Init();
        }
        public StringScanner(string text)
        {
            Text = text;
            Init();
        }
        protected void Init()
        {
            IgnoreWhitespace = true;
        }
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                Length = value.Length;
                Reset();
            }
        }
        public bool IgnoreWhitespace { get; set; }

        protected string _Text;
        public int Length
        { get; protected set; }
        public int Pos
        {
            get;
            set;
        }
        public int LastPos
        {
            get;
            set;
        }
        /// <summary>
        /// Characters that will cause parsing to stop with a fail condition
        /// </summary>
        public string StopChars
        {
            get
            {
                return LookupsToString(_StopChars);
            }
            set
            {
                _StopChars = AddLookups(value);
            }
        }
        public string SeekChars
        {
            get
            {
                return LookupsToString(_SeekChars);
            }
            set
            {
                _SeekChars = AddLookups(value);
            }
        }
        /// <summary>
        /// Causes the next action to permit quoting -- if the first character is a quote character, stop characters between there
        /// and the next matching quote character will be ignored.
        /// </summary>
        public bool AllowQuoting()
        {
            if (IgnoreWhitespace)
            {
                SkipWhitespace();
            }
            if (IsQuote(Peek()))
            {
                Next();
                QuotingActive = true;
                QuoteChar = Current;
            }
            return QuotingActive;
        }
        protected bool QuotingActive
        { get; set; }
        protected char QuoteChar
        { get; set; }
        protected HashSet<char> AddLookups(string charString)
        {
            if (string.IsNullOrEmpty(charString))
            {
                return null;
            }
            HashSet<char> list = new HashSet<char>();
            char[] chars = charString.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                list.Add(chars[i]);
            }
            return list;
        }
        protected string LookupsToString(HashSet<char> list)
        {
            string charString = String.Empty;
            foreach (char item in list)
            {
                charString += item;
            }
            return charString;
        }
        // public bool Matched { get; protected set; }
        // public string LastMatch
        // {get;set;}
        public char Stopper { get; protected set; }
        public char Current
        {
            get
            {
                return Text[Pos];
            }
        }
        public void SkipWhitespace()
        {
            while (!AtEnd && IsWhitespace(Peek()))
            {
                Next();
            }
        }

        public char Peek()
        {
            if (Pos < Length - 2)
            {
                return Text[Pos + 1];
            }
            else
            {
                return (char)0;
            }
        }
        /// <summary>
        /// Moves pointer forward one position
        /// </summary>
        /// <returns></returns>
        public bool Next()
        {
            if (Pos >= Length)
            {
                throw new Exception("Cannot advance beyond end of string.");
            }
            LastPos = Pos;
            Pos++;
            return Pos < Length;
        }
        public bool Prev()
        {
            if (Pos < -1)
            {
                throw new Exception("Cannot reverse beyond beginning of string");
            }
            LastPos = Pos;
            Pos--;
            return Pos >= 0;
        }
        public bool AtEnd
        {
            get
            {
                return Pos == Length || Length == 0;
            }
        }
        /// <summary>
        /// The current character (or next non-whitespace character) must be the parameter value.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public void Expect(char character)
        {
            if (AtEnd)
            {
                ThrowUnexpectedCharacterException();
            }
            int pos = Pos;
            if (Pos == -1)
            {
                Next();
            }
            if (IgnoreWhitespace && character != ' ')
            {
                SkipWhitespace();
            }
            if (Current == character)
            {
                LastPos = pos;
            }
            else
            {
                ThrowUnexpectedCharacterException();
            }
        }
        public void Expect(string charString)
        {
            if (AtEnd)
            {
                ThrowUnexpectedCharacterException();
            }
            int pos = Pos;
            if (Pos == -1)
            {
                Next();
            }
            if (IgnoreWhitespace && charString.IndexOf(' ') < 0)
            {
                SkipWhitespace();
            }
            if (charString.IndexOf(Current) >= 0)
            {
                LastPos = pos;
            }
            else
            {
                ThrowUnexpectedCharacterException();
            }

        }
        public void Reset()
        {
            Pos = -1;
        }
        public void End()
        {
            Pos = Length;
        }
        HashSet<char> _SeekChars = new HashSet<char>();
        HashSet<char> _StopChars = new HashSet<char>();


        /// <summary>
        /// Seek until a stop character is found.
        /// </summary>
        /// <returns></returns>
        public string Seek()
        {
            return Seek(null);
        }
        /// <summary>
        /// Seeks until a stop character is found. If a specific seek characters are provided, will require that these be found first.
        /// </summary>
        /// <returns></returns>
        public string Seek(string seekChars)
        {
            SeekChars = seekChars;
            LastPos = Pos;
            // bool success=false;
            char current;
            string result = String.Empty;
            bool quoting = QuotingActive;
            // When quoting is active, seek/stop characters are ignored until the end of the quoting. Only the matching part inside
            // quotes is returned. When the quote closing is finished, any characters found that are not a "seek" character
            // or whitespace will cause a fail.

            if (quoting)
            {
                while (Next())
                {
                    current = Current;
                    if (QuotingActive)
                    {
                        if (current == '\\')
                        {
                            if (!Next())
                            {
                                ThrowUnexpectedCharacterException();
                            }
                            char newVal;
                            if (TryParseEscapeChar(current, out newVal))
                            {
                                current = newVal;
                                continue;
                            }
                            else
                            {
                                ThrowUnexpectedCharacterException();
                            }
                        }
                        else if (current == QuoteChar)
                        {
                            QuotingActive = false;
                            continue;
                        }
                        result += current;
                    }
                    else
                    {
                        if (IsWhitespace(current) || _SeekChars.Contains(current))
                        {
                            break;
                        }
                        else
                        {
                            ThrowUnexpectedCharacterException("Unexpected character found after quoted text");
                        }
                    }

                }
            }
            else
            {
                // Normal scanning process
                while (Next())
                {
                    current = Current;

                    if (_SeekChars != null && _SeekChars.Contains(current))
                    {
                        break;
                    }
                    if (_StopChars.Contains(current))
                    {

                        if (_SeekChars != null)
                        {
                            ThrowUnexpectedCharacterException("Did not find a seek target before a stopper.");
                        }
                        Stopper = current;
                        break;
                    }
                    result += current;
                }
            }
            return result;
        }
        protected bool IsWhitespace(char character)
        {
            return character == ' ';
        }
        protected bool IsQuote(char character)
        {
            return character == '"' || character == '\'';
        }
        protected bool TryParseEscapeChar(char character, out char newValue)
        {
            switch (character)
            {
                case '\\':
                case '"':
                case '\'':
                    newValue = character;
                    break;
                case 'n':
                    newValue = '\n';
                    break;
                default:
                    newValue = ' ';
                    return false;
            }
            return true;
        }
        public void ThrowUnexpectedCharacterException()
        {
            ThrowUnexpectedCharacterException(null);
        }
        public void ThrowUnexpectedCharacterException(string description)
        {
            string error = !String.IsNullOrEmpty(description) ? description : "Unexpected character found";
            error += " at position " + Pos + ": \"";
            if (LastPos > 0)
            {
                error += ".. ";
            }
            error += Text.SubstringBetween(LastPos<0 ?0:LastPos, Pos) + "\"";

            throw new Exception(error);
        }
    }


}