using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery.Utility.StringScanner.Implementation;

namespace CsQuery.Utility.StringScanner.Patterns
{
    
    public class Number: ExpectPattern
    {
        
        /// <summary>
        /// Normally true
        /// </summary>
        public bool RequireWhitespaceTerminator
        {
            get;
            set;
        }
        public override void Initialize(int startIndex, char[] sourceText)
        {
             base.Initialize(startIndex, sourceText);
             decimalYet = false;
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
            if (EndIndex > Length || EndIndex == StartIndex || failed)
            {
                Result = "";
                return false;
            }
         
            Result = GetOuput(StartIndex, EndIndex, false,false);
            return true;
        }

        protected bool failed = false;
        protected bool decimalYet = false;
        protected virtual bool Expect(ref int index, char current)
        {
            info.Target = current;
            if (index == StartIndex)
            {
                if (!info.Numeric && current!='-')
                {
                    failed=true;
                    return false;
                }
            }
            else
            {
                if (info.Whitespace || info.Operator)
                {
                    return false;
                } else if (current == '.') {
                    if (decimalYet)
                    {
                        failed = true;
                        return false;
                    }
                    else
                    {
                        decimalYet = true;
                    }
                } 
                else if (!info.Numeric)
                {
                    failed = RequireWhitespaceTerminator;
                    return false;
                } 
            }
            index++;
            return true;
        }
    }
}
