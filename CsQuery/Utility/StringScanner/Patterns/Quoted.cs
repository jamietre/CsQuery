using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.Utility.StringScanner.Implementation;

namespace CsQuery.Utility.StringScanner.Patterns
{
    
    public class Quoted: ExpectPattern
    {


        char quoteChar;
        public override void Initialize(int startIndex, char[] sourceText)
        {
            base.Initialize(startIndex, sourceText);
        }
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
