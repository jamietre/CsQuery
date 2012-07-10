using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.StringScanner.Implementation;

namespace CsQuery.StringScanner.Patterns
{
    /// <summary>
    /// A pattern that expects a quoted string. Allows any characters inside the quoted text,
    /// including backslashed escape characters, and terminates upon a matching closing quote.
    /// </summary>

    public class Quoted: ExpectPattern
    {
        /// <summary>
        /// The quote character that was used to open the string.
        /// </summary>
        
        char quoteChar;


        public override bool Validate()
        {
            int index = StartIndex;
            while (index<Source.Length && Expect(ref index, Source[index]))
            {
                ;
            }
            EndIndex = index;
    
            // should not have passed the end
            if (EndIndex > Length || EndIndex == StartIndex)
            {
                Result = "";
                return false;
            }
            return FinishValidate();
        }
        protected virtual bool FinishValidate(){ 
            //return the substring excluding the quotes
            Result = GetOuput(StartIndex, EndIndex, true, true);
            return true;
        }

        protected virtual bool Expect(ref int index, char current)
        {
            info.Target = current;
            if (index == StartIndex)
            {
                quoteChar = current;
                if (!info.Quote)
                {
                    return false;
                }
            }
            else
            {

                bool isEscaped = Source[index - 1] == '\\';
                if (current == quoteChar && !isEscaped)
                {
                    index++;
                    return false;
                }
            }
            index++;
            return true;
        }
    }
}
