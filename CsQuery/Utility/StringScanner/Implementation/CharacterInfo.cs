using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.StringScanner.Implementation
{
    public class CharacterInfo : ICharacterInfo
    {
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
        /// <summary>
        /// Flags indicating the use of this character
        /// </summary>
        public CharacterType Type
        {
            get
            {
                return CharacterData.GetType(Target);
            }
        }
        public bool Alpha
        {
            get
            {
                return CharacterData.IsType(Target,CharacterType.Alpha);
            }
        }
        public bool Numeric
        {
            get
            {
                return CharacterData.IsType(Target,CharacterType.Number);
            }
        }
        /// <summary>
        /// Returns true if numeric, dot or -
        /// </summary>
        public bool NumericExtended
        {
            get
            {
                return CharacterData.IsType(Target, CharacterType.NumberPart);
            }
        }
        public bool Lower
        {
            get
            {
                return CharacterData.IsType(Target, CharacterType.Lower);
            }
        }
        public bool Upper
        {
            get
            {
                return CharacterData.IsType(Target, CharacterType.Upper);
            }
        }
        public bool Whitespace
        {
            get
            {
                return CharacterData.IsType(Target,CharacterType.Whitespace);
            }
        }
        public bool Alphanumeric
        {
            get
            {
                return CharacterData.IsType(Target, CharacterType.Alpha | CharacterType.Number);
            }
        }
        
        public bool Operator
        {
            get
            {
                return CharacterData.IsType(Target, CharacterType.Operator);
            }
        }
        /// <summary>
        /// Enclosing, plus double and single quotes
        /// </summary>
        public bool Bound
        {
            get
            {
                return CharacterData.IsType(Target, CharacterType.Enclosing | CharacterType.Quote);
            }
        }
        /// <summary>
        /// ()[]{}<>`´“”«»
        /// </summary>
        public bool Enclosing
        {
            get
            {
                return CharacterData.IsType(Target, CharacterType.Enclosing);
            }
        }

        public bool Quote
        {
            get
            {
                return CharacterData.IsType(Target, CharacterType.Quote);
            }
        }
        public bool Parenthesis
        {
            get
            {
                return Target == '(' || Target == ')';
            }
        }
        public bool Separator
        {
            get
            {
                return CharacterData.IsType(Target, CharacterType.Separator);
            }
        }
        public bool AlphaISO10646
        {
            get
            {
                return CharacterData.IsType(Target,CharacterType.AlphaISO10646);
            }
        }

        public override string ToString()
        {
            return Target.ToString();
        }
    }

}


