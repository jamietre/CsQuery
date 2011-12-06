using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.StringScanner
{
    public abstract class ExpectPattern: IExpectPattern 
    {
        protected ICharacterInfo info = new CharacterInfo();
        protected char[] Source;
        protected int Start;
        protected int Length;
        public virtual void Initialize(int startIndex, char[] sourceText)
        {
            Source = sourceText;
            Start = startIndex;
            Length = Source.Length;
        }
        /// <summary>
        /// By default assigns current to info.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public virtual bool Expect(ref int index, char current)
        {
            info.Target = current;
            return true;
        }
        /// <summary>
        /// By default, returns true if the string is not empty
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parsedResult"></param>
        /// <returns></returns>
        public virtual bool Validate(int endIndex, out string parsedResult)
        {
            if (endIndex > Start) {
                parsedResult = GetOuput(Start, endIndex,false);
                return true;
            } else {
                parsedResult = "";
                return false;
            }
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
        /// <summary>
        /// Copy the source to an output string betweem startIndex and endIndex, optionally unescaping part of it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="quotedStartIndex"></param>
        /// <param name="quotedEndIndex"></param>
        /// <returns></returns>
        protected string GetOuput(int startIndex, int endIndex, bool honorQuotes)
        {
            bool quoted = false;
            char quoteChar=(char)0;
            StringBuilder sb = new StringBuilder();
            int index=startIndex;
            
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
                                throw new Exception("Invalid escape character found in quoted string: '" + current + "'");
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
