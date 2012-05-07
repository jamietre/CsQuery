using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;

namespace CsQuery.Utility.StringScanner.Patterns
{
    /// <summary>
    /// Match an attribute value. Should be quoted but doesn't have to be.
    /// </summary>
    public class OptionallyQuoted : Quoted
    {
        public OptionallyQuoted()
        {
            Terminators = "])}";
        }
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

        public override bool Validate()
        {
            isQuoted = CharacterData.IsType(Source[StartIndex], CharacterType.Quote);
            return base.Validate();
        }
        protected override bool FinishValidate()
        {
            if (isQuoted)
            {
                return base.FinishValidate();
            }
            else
            {
                Result = GetOuput(StartIndex, EndIndex, false);
                return true;
            }
           
        }
        protected override bool Expect(ref int index, char current)
        {

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
                        index+=1;
                        return false;
                    }
                }
                index++;
                return true;
            }
                
        }

    }
}
