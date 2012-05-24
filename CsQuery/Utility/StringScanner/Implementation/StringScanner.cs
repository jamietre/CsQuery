using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Utility.StringScanner.Implementation
{
    
    // Not implemented - intended to update the scanning code in Selector engine and maybe the HTML parser
    public class StringScanner: IStringScanner
    {
        #region constructors
        public StringScanner()
        {
            Init();
        }
        public StringScanner(string text)
        {
            Text = text;
            Init();
        }
        public static implicit operator StringScanner(string text) {
            return new StringScanner(text);
        }

        protected void Init()
        {
            IgnoreWhitespace = true;
            Reset();
        }
        #endregion

        #region private fields
        private string _Text;
        private string _CurrentMatch;
        private string _LastMatch;
        
        protected CharacterInfo _characterInfo;
        #endregion
        
        #region protected properties
        /// <summary>
        /// When true, the next seek should honor quotes
        /// </summary>
        protected bool QuotingActive
        { get; set; }
        protected char QuoteChar
        { get; set; }
        protected CharacterInfo characterInfo
        {
            get
            {
                if (_characterInfo == null)
                {
                    _characterInfo = new CharacterInfo();
                }
                return _characterInfo;
            }
        }
        protected bool SuppressErrors { get; set; }
        protected char[] _Chars;
        #endregion

        #region public properties
        public string Text
        {
            get
            {
                if (_Text == null)
                {
                    _Text = new string(_Chars);
                }
                return _Text;
            }
            set
            {
                _Text = value ?? "";
                _Chars = null;
                Length = _Text.Length;
                Reset();
            }
        }
        public char[] Chars
        {
            get
            {
                if (_Chars == null)
                {
                    _Chars = _Text.ToCharArray();
                }
                return _Chars;
            }
            set
            {
                _Chars = value;
                _Text=null;
                Length = value.Length;
                Reset();
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
                NextNonWhitespace();
            }
            if (CharacterData.IsType(Peek(),CharacterType.Whitespace))
            {
                Next();
                QuotingActive = true;
                QuoteChar = NextChar;
            }
            return QuotingActive;
        }
        public bool IgnoreWhitespace { get; set; }
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
        /// <summary>
        /// The character at the current scanning position
        /// </summary>
        public char NextChar
        {
            get
            {
                return Text[Pos];
            }
        }
        public string NextCharOrEmpty
        {
            get
            {
                return Finished ? null : NextChar.ToString();
            }
        }
        /// <summary>
        /// The string or character that has been matched
        /// </summary>
        public string Match
        {
            get
            {
                return _CurrentMatch;
            }
            protected set
            {
                _LastMatch = _CurrentMatch;
                _CurrentMatch = value;
            }
        }
        /// <summary>
        /// The string or character matched prior to last operation
        /// </summary>
        public string LastMatch
        {
            get
            {
                return _LastMatch;
            }
            protected set
            {
                _LastMatch = value;
            }
        }
        /// <summary>
        /// The current position is after the last character
        /// </summary>
        public bool Finished
        {
            get
            {
                return Pos >= Length || Length == 0;
            }
        }
        /// <summary>
        /// The current position is on the last character
        /// </summary>
        public bool AtEnd
        {
            get
            {
                return Pos == Length - 1;
            }
        }
        public string LastError
        {
            get;
            protected set;
        }
        public bool Success
        {
            get;
            protected set;
        }
        /// <summary>
        /// The character at the current position is alphabetic
        /// </summary>
        public ICharacterInfo Info
        {
            get
            {
                characterInfo.Target = Finished ? (char)0 : NextChar;
                return characterInfo;
            }
        }
        #endregion
        
        #region public methods
        /// <summary>
        /// Creates a new stringscanner instance from the current match
        /// </summary>
        /// <returns></returns>
        public IStringScanner ToNewScanner()
        {
            if (!Success)
            {
                throw new InvalidOperationException("The last operation was not successful; a new string scanner cannot be created.");
            }
            return Scanner.Create(Match);
        }
        /// <summary>
        /// Creates a new stringscanner instance from the current match, formatted using passed format first.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public IStringScanner ToNewScanner(string format)
        {
            if (!Success)
            {
                throw new InvalidOperationException("The last operation was not successful; a new string scanner cannot be created.");
            }
            return Scanner.Create(String.Format(format,Match));
        }
        /// <summary>
        /// returns true of the text starting at the current position matches the passed text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool Is(string text)
        {
            return text.Length + Pos < Length && Text.Substring(Pos, text.Length) == text;
        }
        public bool IsOneOf(params string[] text)
        {
            return IsOneOf((IEnumerable<string>)(text));
        }
        public bool IsOneOf(IEnumerable<string> text)
        {
            foreach (string val in text)
            {
                if (Is(val))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// If the current character is whitespace, advances to the next non whitespace. Otherwise, nothing happens.
        /// </summary>
        public void SkipWhitespace()
        {
            CachePos();
            AutoSkipWhitespace();
            NewPos();
        }
        protected void SkipWhitespaceImpl()
        {
            if (Finished)
            {
                return;
            }
            if (CharacterData.IsType(NextChar,CharacterType.Whitespace))
            {
                while (!Finished && CharacterData.IsType(NextChar,CharacterType.Whitespace))
                {
                    Next(1);
                }
            }
        }
        protected void AutoSkipWhitespace()
        {
            if (IgnoreWhitespace)
            {
                SkipWhitespaceImpl();
            }
        }
        /// <summary>
        /// Advances to the next non-whitespace character
        /// </summary>
        public void NextNonWhitespace()
        {
            CachePos();
            NextNonWhitespaceImpl();
            NewPos();
        }
        protected void NextNonWhitespaceImpl()
        {
            Next(1);
            SkipWhitespaceImpl();
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
        /// Moves pointer forward one character, or to the position after the next match.
        /// </summary>
        /// <returns></returns>

        public bool Next(int count=1)
        {
            if (Pos >= Length)
            {
                ThrowException("Cannot advance beyond end of string.");
            }

            Pos += count;
            return Pos < Length;
        }
        /// <summary>
        /// Returns to the state before the last Expect. This is not affected by manual Next/Prev operations
        /// </summary>
        /// <returns></returns>
        public void Undo()
        {
            if (LastPos < 0)
            {
                ThrowException("Can't undo - there's nothing to undo");
            }
            Pos = LastPos;
            Match = LastMatch;
            LastMatch = "";
            LastPos = -1;

            NewPos();
        }

        public bool Prev(int count=1)
        {
            if (Pos == 0)
            {
                throw new InvalidOperationException("Cannot reverse beyond beginning of string");
            }
            Pos-=count;
            return Pos >= 0;
        }
        public void AssertFinished()
        {
            if (!Finished)
            {
                ThrowUnexpectedCharacterException();
            }
        }
        public void AssertNotFinished()
        {
            if (Finished)
            {
                ThrowUnexpectedCharacterException();
            }
        }
        public void Reset()
        {
            Pos = 0;
            LastPos = -1;
            Match = "";
            LastError = "";
            Success = true;
        }
        public void End()
        {
            CachePos();
            NewPos(Length);
        }


        public IStringScanner Expect(string text)
        {
            AssertNotFinished();
            CachePos();
            AutoSkipWhitespace();
            if (Is(text))
            {
                Match = Text.Substring(Pos, text.Length);
                NewPos(Pos+text.Length);
            }
            else
            {
                ThrowUnexpectedCharacterException();
            }
            return this;
        }
        public string Get(params string[] values)
        {
            Expect((string[])values);
            return Match;
        }
        public void Expect(params string[] values)
        {
            Expect((IEnumerable<string>)values);
        }
        
        public string Get(IEnumerable<string> stringList)
        {
            Expect(stringList);
            return Match;
        }
        public bool TryGet(IEnumerable<string> stringList, out string result)
        {
            return TryWrapper(()=> {
                Expect(stringList);
            },out result);
        }

        public void Expect(IEnumerable<string> stringList)
        {
            AssertNotFinished();
            CachePos();
            AutoSkipWhitespace();
            string startChars = "";
            foreach (string expected in stringList)
            {
                startChars+=expected[0];
            }
            if (ExpectCharImpl(startChars))
            {
                foreach (string expected in stringList)
                {
                    if (Is(expected))
                    {
                        Match = Text.Substring(Pos, expected.Length);
                        NewPos(Pos + expected.Length);
                        return;
                    }
                }
            }
            ThrowUnexpectedCharacterException();
        }

        public char GetChar(char character) {
            ExpectChar(character);
            return Match[0];
        }
        public bool TryGetChar(char character, out string result)
        {
            return TryWrapper(() =>
            {
                ExpectChar(character);
            }, out result);
        }
        /// <summary>
        /// If current character (or next non-whitespace character) is not the expected value, then an error is thrown
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        /// 
        public IStringScanner ExpectChar(char character)
        {
            AssertNotFinished();
            CachePos();
            AutoSkipWhitespace();

            if (NextChar == character)
            {
                Match = NextChar.ToString();
                Next(1);
                NewPos();
            }
            else
            {
                ThrowUnexpectedCharacterException();
            }
            return this;
        }
        public char GetChar(string characters)
        {
            return GetChar(characters.ToCharArray());
        }
        public bool TryGetChar(string characters, out string result)
        {
            return TryWrapper(() =>
            {
                Expect(characters);
            }, out result);
        }
        public IStringScanner ExpectChar(string characters)
        {
            return ExpectChar(characters.ToCharArray());
            
        }
        public char GetChar(params char[] characters)
        {
            return GetChar((IEnumerable<char>)characters);
        }
        public IStringScanner ExpectChar(params char[] characters)
        {
            return ExpectChar((IEnumerable<char>)characters);
        }
        public char GetChar(IEnumerable<char> characters)
        {
            ExpectChar((IEnumerable<char>)characters);
            return Match[0];
        }
        public bool TryGetChar(IEnumerable<char> characters, out string result)
        {
            return TryWrapper(() =>
            {
                ExpectChar((IEnumerable<char>)characters);
            }, out result);
        }
        /// If one of the current characters (or next non-whitespace character) is not the expected value, then an error is thrown
        public IStringScanner ExpectChar(IEnumerable<char> characters)
        {
            AssertNotFinished();
            CachePos();
            AutoSkipWhitespace();
            if (ExpectCharImpl(characters))
            {
                Match = NextChar.ToString();
                Next(1);
                NewPos();
            }
            else
            {
                ThrowUnexpectedCharacterException();
            }
            return this;
        }
        protected bool ExpectCharImpl(IEnumerable<char> characters)
        {
            //HashSet<char> expected = new HashSet<char>(characters);
            foreach (char item in characters) {
                if (item==NextChar) {
                    return true;
                }
            }
            return false;

        }
        

        public string GetNumber()
        {
            ExpectNumber();
            return Match;
        }
        public bool TryGetNumber(out string result)
        {
            return TryWrapper(() =>
            {
                ExpectNumber();
            }, out result);
        }
        public bool TryGetNumber<T>(out T result) where T:IConvertible 
        {
            string stringResult;
            bool gotNumber = TryWrapper(() =>
            {
                ExpectNumber();
            }, out stringResult);

            if (gotNumber)
            {
                result = (T)Convert.ChangeType(stringResult, typeof(T));
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }
        public bool TryGetNumber(out int result)
        {
            double doubleResult;
            if (TryGetNumber(out doubleResult))
            {
                result = Convert.ToInt32(doubleResult);
                return true;
            }
            else
            {
                result = 0;
                return false;
            }
        }
        /// <summary>
        /// Starting with the current character, treats text as a number, seeking until the next character that would terminate a valid number.
        /// </summary>
        /// <returns></returns>
        public IStringScanner ExpectNumber()
        {
            return Expect(MatchFunctions.Number());
        }
        public bool TryGetAlpha(out string result)
        {
            return TryWrapper(() =>
            {
                GetAlpha();
            }, out result);
        }
        public string GetAlpha()
        {
            ExpectAlpha();
            return Match;
        }

        /// <summary>
        /// Starting with the current character, seeks until a non-alpha character is found
        /// </summary>
        /// <returns></returns>
        public IStringScanner ExpectAlpha()
        {
            return Expect(MatchFunctions.Alpha);
            
        }

        public string Get(IExpectPattern pattern)
        {
            ExpectImpl(pattern, true);
            return Match;
        }
        public bool TryGet(IExpectPattern pattern, out string result)
        {
            return TryWrapper(() =>
            {
                Expect(pattern);
            }, out result);
        }

        /// <summary>
        /// Continue seeking as long as the delegate returns true.
        /// </summary>
        /// <param name="validate">
        /// A function accepting parameters int, CharacterInfo, char[] and returning bool.
        /// int is the index of the matching string starting with 0
        /// CharacterInfo is a wrapper for the current character
        /// char[] is the remainder of the string starting at Pos
        /// </param>
        public IStringScanner Expect(IExpectPattern pattern)
        {
            ExpectImpl(pattern, true);
            return this;
        }
        public string Get(Func<int, char, bool> validate)
        {
            Expect(validate);
            return Match;
        }
        public bool TryGet(Func<int, char, bool> validate, out string result)
        {
            return TryWrapper(() =>
            {
                Expect(validate);
            }, out result);
        }
        /// <summary>
        /// Continue seeking as long as the delegate returns True
        /// </summary>
        /// <param name="del"></param>
        public IStringScanner Expect(Func<int, char, bool> validate)
        {
            AssertNotFinished();
            CachePos();
            AutoSkipWhitespace();
            int startPos = Pos;
            int index = 0;
            while (!Finished && validate(index, NextChar))
            {
                Pos++;
                index++;
            }
            if (Pos > startPos)
            {
                Match = Text.SubstringBetween(startPos, Pos);
                NewPos();
            }
            else
            {
                ThrowUnexpectedCharacterException();
            }
            return this;
        }
        public string GetBoundedBy(string start, string end, bool allowQuoting=false)
        {
            ExpectBoundedBy(start, end);
            return Match;
        }
        public bool TryGetBoundedBy(string start, string end, bool allowQuoting, out string result)
        {
            return TryWrapper(() =>
            {
                ExpectBoundedBy(start,end,allowQuoting);
            }, out result);
        }
        public IStringScanner ExpectBoundedBy(string start, string end, bool allowQuoting = false)
        {
            var boundedBy = new Patterns.Bounded();
            boundedBy.BoundStart = start;
            boundedBy.BoundEnd = end;
            boundedBy.HonorInnerQuotes=allowQuoting;
            return Expect(boundedBy);
        }
        /// <summary>
        /// The single character bound will be matched with a closing char for () [] {} &lt;&gt; or the same char for anything else
        /// </summary>
        /// <param name="bound"></param>
        public string GetBoundedBy(char bound, bool allowQuoting=false)
        {
            ExpectBoundedBy(bound);
            return Match;
        }
        public IStringScanner ExpectBoundedBy(char bound, bool allowQuoting = false)
        {
            var boundedBy = new Patterns.Bounded();
            boundedBy.BoundStart = bound.ToString();
            boundedBy.HonorInnerQuotes = allowQuoting;
            return Expect(boundedBy);
        }
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// The implementation - if the 2nd parm is false, it is the opposite (seek until the match condition is met)
        /// 2nd parm NOT IMPLEMENTED
        /// </summary>
        /// <param name="validate"></param>
        /// <param name="untilTrue"></param>
        protected StringScanner ExpectImpl(IExpectPattern pattern, bool untilTrue)
        {
            AssertNotFinished();
            CachePos();
            AutoSkipWhitespace();
            int startPos = Pos;

            pattern.Initialize(Pos, Chars);
            // call the function one more time after the end of the string - this determines outcome
            if (pattern.Validate())
            {
                Match = pattern.Result;
                NewPos(pattern.EndIndex);
            }
            else
            {
                Pos = pattern.EndIndex; // for error report to be accurate - will be undone at end
                ThrowUnexpectedCharacterException();
            }
            return this;
        }
        protected bool TryWrapper(Action action, out string result)
        {
            SuppressErrors = true;
            action();
            SuppressErrors = false;
            if (Success)
            {
                result = Match;
            }
            else
            {
                result = "";
            }
            return Success;
        }

        [DebuggerStepThrough]
        protected void ThrowUnexpectedCharacterException()
        {
            if (Pos >= Length)
            {
                ThrowUnexpectedEndOfStringException();
            }
            else
            {
                ThrowException("Unexpected character found",Pos);
            }
        }
        [DebuggerStepThrough]
        protected void ThrowUnexpectedEndOfStringException()
        {
            ThrowException("The string unexpectedly ended",Pos);
        }
        [DebuggerStepThrough]
        protected void ThrowException(string message)
        {
            ThrowException(message, -1);
        }
        //[DebuggerStepThrough]
        protected void ThrowException(string message, int errPos)
        {
            string error = message;
            int pos = -1;
            if (String.IsNullOrEmpty(Text))
            {
                error = " -- the string is empty.";
            }
            else
            {
                pos = Math.Min(errPos + 1, Length - 1);
            }

            RestorePos();

            if (pos >= 0)
            {
                error += " at position " + pos + ": \"";

                if (Pos != pos)
                {
                    if (Pos > 0 && Pos < Length)
                    {
                        error += ".. ";
                    }
                    error += Text.SubstringBetween(Math.Max(Pos - 10, 0), pos) + ">>" + Text[pos] + "<<";
                    if (pos < Length - 1)
                    {
                        error += Text.SubstringBetween(pos + 1, Math.Min(Length, pos + 30));
                    }
                    error += "\"";
                }
            }
            
            LastError = error;

            if (SuppressErrors)
            {
                Success = false;
            }
            else
            {
                throw new InvalidOperationException(error);
            }
        }
        #endregion
        
        #region private methods
        /// <summary>
        /// Caches the current position
        /// </summary>
        private int cachedPos;
        private string cachedMatch;
        bool cached = false;
        /// <summary>
        /// Cache the last pos before an attempted operation,
        /// </summary>
        protected void CachePos()
        {
            LastError = "";
            Success = true;
            if (cached)
            {
                throw new InvalidOperationException("Internal error: already cached");
            }
            cached = true;
            cachedPos = Pos;
            cachedMatch = Match;
        }
        /// <summary>
        /// Sets the current position, updates the last pos from cache, and clears any current match. If the cached position is the same
        /// as the current position, nothing is done.
        /// </summary>
        protected void NewPos(int pos)
        {
            Pos = pos;
            NewPos();
        }
        protected void NewPos()
        {
            if (Pos != cachedPos)
            {
                LastPos = cachedPos;
                LastMatch = cachedMatch;
            }
            cached = false;
            
        }
        /// <summary>
        /// Restores position from cache
        /// </summary>
        protected void RestorePos()
        {
            if (cached)
            {
                Pos = cachedPos;
                Match= cachedMatch;
                cached = false;
            }
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

      
        
        #endregion
    }


}