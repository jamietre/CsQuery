using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.StringScanner
{
    

    public class CharacterInfo : ICharacterInfo
    {
        private HashSet<char> whitespace = new HashSet<char>(new char[] { '\x0020', '\x0009', '\x000A', '\x000C', '\x000D' });

        public CharacterInfo()
        {

        }
        public CharacterInfo(char character)
        {
            Target = character;
        }
        public static implicit operator CharacterInfo(char character) {
            return new CharacterInfo(character);
        }
        public static ICharacterInfo Create(char character)
        {
            return new CharacterInfo(character);
        }
        public static char MatchingBound(char character) {
            switch(character) {
                case '"':
                    return '"';
                case '\'':
                    return '\'';
                case '[':
                    return ']';
                case ']':
                    return '[';
                case '(':
                    return ')';
                case ')':
                    return '(';
                case '{':
                    return '}';
                case '}':
                    return '{';
                case '<':
                    return '>';
                case '>':
                    return '<';
                default:
                    return character;
            }

        }
        public char Target { get; set; }

        IConvertible IValueInfo.Target
        {
            get
            {
                return Target;
            }
            set
            {
                Target = (char)value;
            }
        }
        public bool Alpha
        {
            get
            {
                return "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(Target);
            }
        }
        public bool Numeric
        {
            get
            {
                return "0123456789".Contains(Target);
            }
        }
        /// <summary>
        /// Returns true if numeric, dot or -
        /// </summary>
        public bool NumericExtended
        {
            get
            {
                return "0123456789.-".Contains(Target);
            }
        }
        public bool Lower
        {
            get
            {
                return "abcdefghijklmnopqrstuvwxyz".Contains(Target);
            }
        }
        public bool Upper
        {
            get
            {
                return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(Target);
            }
        }
        public bool Whitespace
        {
            get
            {
                return whitespace.Contains(Target);
            }
        }
        public bool Alphanumeric
        {
            get
            {
                return Alpha || Numeric;
            }
        }
        public bool Operator
        {
            get
            {
                return "!+-*/%<>^=".Contains(Target);
            }
        }
        /// <summary>
        /// True when ()[]{}<>
        /// </summary>
        public bool Bound
        {
            get
            {
                return "()[]{}<>\"'".Contains(Target);
            }
        }
        public bool Quote
        {
            get
            {
                return Target == '\'' || Target == '"';
            }
        }
        public bool Parenthesis
        {
            get
            {
                return "()".Contains(Target);
            }
        }
    }

    public class StringInfo : IStringInfo
    {
        public StringInfo()
        {

        }
        public StringInfo(string text)
        {
            Target = text;
        }
        public static implicit operator StringInfo(string text)
        {
            return new StringInfo(text);
        }
        public static StringInfo Create(string text)
        {
            return new StringInfo(text);
        }
        protected CharacterInfo charInfo = new CharacterInfo();
        
        protected bool CheckFor(Func<CharacterInfo,bool> function)
        {
            foreach (char current in Target)
            {
                charInfo.Target = current;
                if (!function(charInfo))
                {
                    return false;
                }
            }
            return true;
        }
        public string Target { 
            get;set;
        }
        
        IConvertible IValueInfo.Target
        {
            get
            {
                return Target;
            }
            set
            {
                Target = (string)value;
            }
        }
        protected Func<CharacterInfo,bool> isAlpha = new Func<CharacterInfo, bool>(item => item.Alpha);
        
        public bool Alpha
        {
            get { return Exists && CheckFor(isAlpha); }
        }

        private static Func<CharacterInfo, bool> isNumeric = new Func<CharacterInfo, bool>(item => item.Numeric);
        public bool Numeric
        {
            get { return Exists && CheckFor(isNumeric);  }
        }

        private static Func<CharacterInfo, bool> isNumericExtended = new Func<CharacterInfo, bool>(item => item.NumericExtended);
        public bool NumericExtended
        {
            get { return Exists && CheckFor(isNumericExtended); }
        }

        private static Func<CharacterInfo, bool> isLower = new Func<CharacterInfo, bool>(item => !item.Alpha || item.Lower);
        public bool Lower
        {
            get { return Exists && HasAlpha && CheckFor(isLower); }
        }

        private static Func<CharacterInfo, bool> isUpper = new Func<CharacterInfo, bool>(item => !item.Alpha || item.Upper);
        public bool Upper
        {
            get { return Exists && HasAlpha && CheckFor(isUpper); }
        }

        private static Func<CharacterInfo, bool> isWhitespace = new Func<CharacterInfo, bool>(item => item.Whitespace);
        public bool Whitespace
        {
            get { return Exists && CheckFor(isWhitespace); }
        }

        private static Func<CharacterInfo, bool> isAlphanumeric = new Func<CharacterInfo, bool>(item => item.Alpha || item.Numeric);
        public bool Alphanumeric
        {
            get { return Exists && CheckFor(isAlphanumeric); }
        }

        protected Func<CharacterInfo, bool> isOperator = new Func<CharacterInfo, bool>(item => item.Operator);
        public bool  Operator
        {
            get { return Exists && CheckFor(isOperator); }
        }

        public bool HasAlpha
        {
            get
            {
                foreach (char current in Target)
                {
                    charInfo.Target = current;
                    if (charInfo.Alpha)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        

        protected Func<CharacterInfo, bool> isParenthesis = new Func<CharacterInfo, bool>(item => item.Parenthesis);
        public bool Parenthesis
        {
            get { return Exists && CheckFor(isParenthesis); }
        }

        protected Func<CharacterInfo, bool> hasAttributeName = new Func<CharacterInfo, bool>(item => item.Parenthesis);
        public bool HtmlAttributeName
        {
            get {
                if (!Exists) return false;
                charInfo.Target = Target[0];

                if (!(charInfo.Alpha || charInfo.Target==':' || charInfo.Target=='_')) {
                    return false;
                }
                for (int i=1;i<Target.Length;i++) {
                    charInfo.Target = Target[i];
                    if (!charInfo.Alphanumeric && !("_:.-".Contains(charInfo.Target))) {
                        return false;
                    }
                    
                }
                return true;
            }
        }
        protected bool Exists{
        get {
            return !String.IsNullOrEmpty(Target);
        }
        }
    }
}


