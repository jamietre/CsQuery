using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.EquationParser.Implementation
{
    public abstract class Operand
    {

        public abstract bool IsInteger { get; }
        public abstract bool IsNumber { get; }
        public abstract bool IsFloatingPoint { get; }
        public abstract bool IsText { get; }
        public abstract bool IsBoolean { get; }
        public abstract TypeCode GetTypeCode();
        public abstract bool ToBoolean(IFormatProvider provider);
        public abstract byte ToByte(IFormatProvider provider);
        public abstract char ToChar(IFormatProvider provider);
        public abstract DateTime ToDateTime(IFormatProvider provider);
        public abstract decimal ToDecimal(IFormatProvider provider);
        public abstract double ToDouble(IFormatProvider provider);
        public abstract short ToInt16(IFormatProvider provider);
        public abstract int ToInt32(IFormatProvider provider);
        public abstract long ToInt64(IFormatProvider provider);
        public abstract sbyte ToSByte(IFormatProvider provider);
        public abstract float ToSingle(IFormatProvider provider);
        public abstract string ToString(IFormatProvider provider);
        public abstract object ToType(Type conversionType, IFormatProvider provider);
        public abstract ushort ToUInt16(IFormatProvider provider);
        public abstract uint ToUInt32(IFormatProvider provider);
        public abstract ulong ToUInt64(IFormatProvider provider);

    }

    public abstract class Operand<T> : Operand, IOperand<T> where T : IConvertible
    {
        public Operand()
        {

            _IsInt = Utils.IsIntegralType<T>();
            _IsNumber = Utils.IsNumericType<T>();
            _IsText = typeof(T) == typeof(string) || typeof(T) == typeof(char);
            _IsBoolean = typeof(T) == typeof(bool);
        }

        private T _Value;
        protected bool? _IsNumber;
        protected bool? _IsInt;
        protected bool? _IsText;
        protected bool? _IsBoolean;

        protected int intValue;
        protected double doubleValue;


        /// <summary>
        /// Indicates that this operand is either an integral type or contains an integral value. 
        /// That is, non-integral types containing integral values will still report true
        /// </summary>
        public override bool IsInteger
        {
            get
            {
                if (_IsInt != null)
                {
                    return (bool)_IsInt;
                }
                else
                {
                    return Utils.IsIntegralValue(Value);
                }
            }
        }
        public override bool IsFloatingPoint
        {
            get
            {
                return IsNumber && !IsInteger;
            }
        }
        public override bool IsNumber
        {
            get
            {
                if (_IsNumber != null)
                {
                    return (bool)_IsNumber;
                }
                else
                {
                    return Utils.IsNumericType<T>();
                }
            }
        }
        public override bool IsText
        {
            get
            {
                if (_IsText != null)
                {
                    return (bool)_IsText;
                }
                else
                {
                    return Value is string;
                }
            }
        }
        public override bool IsBoolean
        {
            get
            {
                if (_IsBoolean != null)
                {
                    return (bool)_IsBoolean;
                }
                else
                {
                    return Value is bool;
                }
            }
        }
        public virtual T Value
        {
            get
            {
                return _Value;
            }
            protected set
            {
                _Value = value;

                //doubleValue = Convert.ToDouble(value);
                //intValue = (int)Math.Floor(doubleValue);
  
                if (!IsInteger)
                {
                    _IsInt = Utils.IsIntegralValue(value);
                }
                //_IsString = null;

            }
        }

        IConvertible IOperand.Value
        {
            get
            {
                return Value;
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #region iconvertible members
        public override TypeCode GetTypeCode()
        {
            return Value.GetTypeCode();
        }

        public override bool ToBoolean(IFormatProvider provider)
        {
            return intValue != 0;
        }

        public override byte ToByte(IFormatProvider provider)
        {
            if (intValue >= 0 || intValue < 255)
            {
                return (byte)intValue;
            }
            return ConversionException<byte>();
        }

        public override char ToChar(IFormatProvider provider)
        {
            if (intValue < 0 || intValue > 65535)
            {
                return (char)intValue;
            }
            return ConversionException<char>();
        }

        public override DateTime ToDateTime(IFormatProvider provider)
        {
            return ConversionException<DateTime>();
        }

        public override decimal ToDecimal(IFormatProvider provider)
        {
            return (decimal)doubleValue;
        }

        public override double ToDouble(IFormatProvider provider)
        {
            return doubleValue;
        }

        public override short ToInt16(IFormatProvider provider)
        {
            if (intValue < Int16.MinValue || intValue > Int16.MaxValue)
            {
                return ConversionException<Int16>();
            }
            return (Int16)intValue;
        }

        public override int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
        protected U ConversionException<U>()
        {
            throw new InvalidCastException("Cannot convert value '" + Value + "' to type " + typeof(U).ToString());
        }
        #endregion

    }
}
