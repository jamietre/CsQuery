using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using CsQuery.StringScanner.Implementation;

// TODO this should be fully commented; however it's not part of the main public API

#pragma warning disable 1591
#pragma warning disable 1570

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
						// process unicode char code point, if presented
						int tempIndex = index;
						StringBuilder sb = new StringBuilder();
						while (Source[tempIndex] != ' ')
						{
							sb.Append(Source[tempIndex]);

							if (tempIndex == Source.Length - 1 ||	// end of string?
								tempIndex - index == 5)				// only 6 hexadecimal digits are allowed
								break;

							tempIndex++;
						}

						if (sb.Length == 2 || sb.Length == 6)
						{
							int value = 0;
							if (Int32.TryParse(sb.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
							{
								character = (char)value;
								index = tempIndex;
							}
						}
												
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
