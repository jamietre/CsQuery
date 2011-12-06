using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.StringScanner
{
    public static class MatchFunctions
    {
        public static bool Alpha(int index, ICharacterInfo info ) {
            return info.Alpha;
        }
        public static bool Numeric(int index, ICharacterInfo info)
        {
            return info.NumericExtended;
        }
        public static bool Alphanumeric(int index, ICharacterInfo info)
        {
            return info.Alphanumeric;
        }
        public static bool HTMLAttribute(int index, ICharacterInfo info)
        {
            if (index == 0)
            {
                return info.Alpha;
            }
            else
            {
                return info.Alphanumeric || "_:.-".Contains(info.Target);
            }
        }
        public static bool HTMLTagName(int index, ICharacterInfo info)
        {
            if (index == 0)
            {
                return info.Alpha;
            }
            else
            {
                return info.Alphanumeric || "_-".Contains(info.Target);
            }
        }

        public static IExpectPattern BoundedBy(string boundStart, string boundEnd=null, bool honorInnerQuotes=false)
        {
            
            var pattern = new Patterns.Bounded();
            pattern.BoundStart = boundStart;
            if (!String.IsNullOrEmpty(boundEnd))
            {
                pattern.BoundEnd= boundEnd;
            }
            pattern.HonorInnerQuotes = honorInnerQuotes;
            return pattern;
        }

        public static IExpectPattern Bounded
        {
            get
            {
                var pattern = new Patterns.Bounded();
                pattern.HonorInnerQuotes = false;
                return pattern;
            }
        }
        public static IExpectPattern BoundedWithQuotedContent
        {
            get
            {
                var pattern = new Patterns.Bounded();
                pattern.HonorInnerQuotes = true;
                return pattern;
            }
        }
        public static bool NonWhitespace(int index, ICharacterInfo info)
        {
            return !info.Whitespace;
        }
        public static bool QuoteChar(int index, ICharacterInfo info) {
            return info.Quote;
        }

        public static bool BoundChar(int index, ICharacterInfo info)
        {
            return info.Bound;
        }

        public static IExpectPattern Quoted
        {
            get
            {
                return new Patterns.Quoted();
            }
        }
        public static bool PseudoSelector(int index, ICharacterInfo info)
        {
            return index == 0 ? info.Alpha : info.Alpha || info.Target == '-';
        }
        public static bool CssClass(int index, ICharacterInfo info)
        {
            if (index == 0)
            {
                return info.Alpha || info.Target == '-' || info.Target == '_';
            }
            else
            {
                return info.Alphanumeric || info.Target == '-';
            }
        }
        public static IExpectPattern OptionallyQuoted
        {
            get
            {
                return new Patterns.OptionallyQuoted();
            }
        }
        public static bool Operator(int index, ICharacterInfo info)
        {
            return info.Operator;
        }

    }

}
