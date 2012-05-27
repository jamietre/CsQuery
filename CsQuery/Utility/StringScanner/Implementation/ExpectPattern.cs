using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.StringScanner.Implementation
{
    public abstract class ExpectPattern: IExpectPattern 
    {
        protected ICharacterInfo info = CharacterData.CreateCharacterInfo();
        protected char[] Source;
        protected int StartIndex;
        protected int Length;
        public virtual void Initialize(int startIndex, char[] sourceText)
        {
            Source = sourceText;
            StartIndex = startIndex;
            Length = Source.Length;
        }

        /// <summary>
        /// By default, returns true if the string is not empty
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parsedResult"></param>
        /// <returns></returns>
        public virtual bool Validate()
        {
            if (EndIndex > StartIndex)
            {
                Result = GetOuput(StartIndex,EndIndex, false);
                return true;
            } else {
                Result = "";
                return false;
            }
        }
        public virtual int EndIndex
        {
            get;
            protected set;
        }
        public virtual string Result
        {
            get;
            protected set;
        }
        protected char[] ExtractSourceSubstring(int startIndex, int endIndex)
        {
            char[] output = new char[endIndex - startIndex];
            for (int i = startIndex, j=0; i < endIndex; i++, j++)
            {
                output[j] = Source[i];
            }
            return output;
        }
        protected bool MatchSubstring(int startIndex, string substring)
        {
            if (startIndex + substring.Length <= Source.Length)
            {
                for (int i = 0; i < substring.Length; i++)
                {
                    if (Source[startIndex + i] != substring[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        protected string GetOuput(int startIndex, int endIndex, bool honorQuotes)
        {
            return GetOuput(startIndex, endIndex, honorQuotes, false);
        }
        /// <summary>
        /// Copy the source to an output string betweem startIndex and endIndex, optionally unescaping part of it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="quotedStartIndex"></param>
        /// <param name="quotedEndIndex"></param>
        /// <returns></returns>
        protected string GetOuput(int startIndex, int endIndex, bool honorQuotes, bool stripQuotes)
        {
            bool quoted = false;

            char quoteChar=(char)0;
            StringBuilder sb = new StringBuilder();
            int index=startIndex;
            if (endIndex <= index) {
                return "";
            }
            if (stripQuotes && CharacterData.IsType(Source[index], CharacterType.Quote))
            {
                quoted = true;
                quoteChar = Source[index];
                index++;
                endIndex--;
            }
            while (index<endIndex)
            {
                char current = Source[index];
                info.Target = current;
                if (honorQuotes)
                {
                    if (!quoted)
                    {
                        if (info.Quote)
                        {
                            quoted = true;
                            quoteChar = current;
                        }
                    }
                    else
                    {
                        if (current == quoteChar)
                        {
                            quoted = false;
                        }
                        if (current == '\\')
                        {
                            char newChar;
                            index++;
                            if (TryParseEscapeChar(Source[index], out newChar))
                            {
                                current = newChar;
                            }
                            else
                            {
                                throw new InvalidOperationException("Invalid escape character found in quoted string: '" + current + "'");
                            }
                        }
                    }
                }
                sb.Append(current);
                index++;
            }
            return sb.ToString();
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
    }
}
