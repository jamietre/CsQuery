using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery.Utility.StringScanner.Patterns
{
    
    public class Quoted: ExpectPattern
    {
        //bool matched;
        char quoteChar;
        public override bool Expect(ref int index, char current)
        {
            base.Expect(ref index,current);
            if (index==Start) {
                quoteChar = current;
                if (!info.Quote)
                {
                    return false;
                }
            } else {
                
                bool isEscaped = Source[index-1]=='\\';
                if (current == quoteChar && !isEscaped)
                {
                    index++;
                    return false;
                }
            }
            index++;
            return true;
        }
        public override bool Validate(int endIndex,out string result)
        {
            // should not have passed the end
            if (endIndex > Length || endIndex==Start)
            {
                result = "";
                return false;
            }
            //return the substring excluding the quotes
            result = GetOuput(Start, endIndex, true);
            result = result.SubstringBetween(1, result.Length - 1);
            return true;
        }

       
    }
}
