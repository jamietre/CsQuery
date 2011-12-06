using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.StringScanner.Patterns
{
    /// <summary>
    /// Match an attribute value. Should be quoted but doesn't have to be.
    /// </summary>
    public class OptionallyQuoted : Quoted
    {
        bool isQuoted;

        public override void Initialize(int startIndex, char[] sourceText)
        {
            base.Initialize(startIndex, sourceText);
            isQuoted = false;
        }
        /// <summary>
        /// When unquoted, this will terminate the string
        /// </summary>
        public IEnumerable<char> Terminators
        {
            get;
            set;
        }

        public override bool Expect(ref int index, char current)
        {
            info.Target = current;
            if (index ==Start && info.Quote) {
                isQuoted=true;
            }
            if (isQuoted)
            {
                return base.Expect(ref index, current);
            }
            else
            {
                foreach (char item in Terminators)
                {
                    if (current == item)
                    {
                        return false;
                    }
                }
                index++;
                return true;
            }
                
        }
        public override bool Validate(int endIndex, out string result)
        {
            if (isQuoted)
            {
                return base.Validate(endIndex, out result);
            } else {
                result = GetOuput(Start, endIndex, false);
                return true;
            }
        }

    }
}
