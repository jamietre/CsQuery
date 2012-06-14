using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.StringScanner.Implementation;

namespace CsQuery.StringScanner.Patterns
{
    /// <summary>
    /// Match a string pattern against a particular character validation function, but allow the backslash to escape 
    /// any character.
    /// </summary>
    public class EscapedString: ExpectPattern
    {
        public EscapedString(Func<int,char, bool> validCharacter)
        {
            ValidCharacter = validCharacter;
        }
        protected Func<int, char, bool> ValidCharacter;
        protected bool Escaped;

        public override bool Validate()
        {
            int index = StartIndex;
            int relativeIndex = 0;
            bool done=false;
            Result="";

            while (index<Source.Length && !done)
            {
                char character = Source[index];
                if (!Escaped && character == '\\')
                {
                    Escaped = true;
                }
                else
                {
                    if (Escaped)
                    {
                        Escaped = false;
                    }
                    else
                    {
                        if (!ValidCharacter(relativeIndex,character))
                        {
                            done = true;
                            continue;
                        }
                    }
                    Result += character;
                }
                index++;
                relativeIndex++;
            }
            bool failed = Escaped;
            EndIndex = index;
    
            // should not have passed the end
            if (EndIndex > Length || EndIndex == StartIndex || failed)
            {
                Result = "";
                return false;
            }
         
            return true;
        }

        protected bool failed = false;
       
    }
}
