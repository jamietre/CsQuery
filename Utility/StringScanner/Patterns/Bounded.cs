using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery.Utility.StringScanner.Patterns
{
    /// <summary>
    /// Matches anything that is bounded by acceped bounding characters
    /// </summary>
    public class Bounded: ExpectPattern
    {
        private string _BoundStart;
        private string _BoundEnd;
        private char _BoundStartChar = (char)0;
        private char _BoundEndChar = (char)0;

        protected bool boundAny = true;
        private bool quoting;
        private char quoteChar;
        private bool matched;
        // This is a one-character type with different open/close - must track nested entites
        private int nestedCount;


        public bool HonorInnerQuotes { get; set; }
        public string BoundStart
        {
            get
            {
                return _BoundStart ?? BoundStartChar.ToString();
            }
            set
            {
                boundAny = false;
                if (value.Length == 0)
                {
                    boundAny = true;
                }
                else
                {
                    BoundStartChar = value[0];
                }
                _BoundStart = value;
            }
        }
        public string BoundEnd
        {
            get
            {
                return _BoundEnd ?? BoundEndChar.ToString();
            }
            set
            {
                _BoundEnd = value;
                _BoundEndChar = value[0];
            }
        }
        
        protected char BoundStartChar
        {
            get
            {
                return _BoundStartChar;
            }
            set
            {
                _BoundStartChar = value;
                BoundEndChar = CharacterInfo.MatchingBound(value);
            }
        }
        protected char BoundEndChar
        {
            get
            {
                return _BoundEndChar;
            }
            set
            {
                _BoundEndChar = value;
            }
        }
  
        public override void Initialize(int startIndex, char[] sourceText)
        {
            base.Initialize(startIndex, sourceText);
          
            nestedCount = 0;
            matched = false;
            quoting = false;
        }
        public override bool Expect(ref int index, char current)
        {
            base.Expect(ref index,current);
            
            if (!quoting) {
                // only try to match bounds when not inside quotes
                if (index == Start)
                {
                    if (boundAny && info.Bound)
                    {
                        BoundStartChar = current;

                        // will not increment if return false (after short-circuit) - causing validation failure
                        // when index==Start
                        return info.Bound && index++ < Length;
                    }
                    else if (MatchSubstring(0, BoundStart))
                    {
                        index += BoundStart.Length;
                        return true;
                    }
                } 
                else if (current==BoundStartChar)
                {
                    if (MatchSubstring(index, BoundStart))
                    {
                        index += BoundStart.Length;
                        nestedCount++;
                    }
                }
                if (current == BoundEndChar )
                {
                    if (boundAny)
                    {
                        if (nestedCount==0) {
                            matched= true;
                            index++;
                            return false;
                        } else {
                            nestedCount--;
                        }
                    } 
                    else if (MatchSubstring(index, BoundEnd))
                    {
                        if (nestedCount==0) {
                            index += BoundEnd.Length;
                            matched = true;
                            return false;
                        } else {
                            nestedCount--;
                        }
                    }
                }
            }
            
            
            // Now the regular part
            if (HonorInnerQuotes)
            {
                if (!quoting)
                {
                    if (info.Quote)
                    {
                        quoting = true;
                        quoteChar = current;
                    }
                }
                else
                {
                    bool isEscaped = Source[index - 1] == '\\';
                    if (current == quoteChar && !isEscaped)
                    {
                        quoting = false;
                    }
                }
            }
            index++;
            return true;
            
        }
        public override bool Validate(int endIndex,out string result)
        {

            // should not have passed the end

            if (endIndex > Length || endIndex==Start || !matched)
            {
                result= "";
                return false;
            }
            
            // HonorQuotes parm is false no matter what because we don't want to process escape characters for this method -only for
            // the actual "Quoted" method
            result = GetOuput(Start+BoundStart.Length, endIndex-BoundEnd.Length, false);
            return true;

        }

    }
}
