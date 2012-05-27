using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser.Implementation
{
    public static class Utils
    {
        public static bool IsIntegralType<T>()
        {
            return IsIntegralType(typeof(T));
        }
        public static bool IsIntegralType(IConvertible value)
        {
            return IsIntegralType(value.GetType());
        }
        public static bool IsIntegralType(Type type)
        {
            return type == typeof(Int16) ||
                type == typeof(Int32) ||
                type == typeof(Int64) ||
                type == typeof(UInt16) ||
                type == typeof(UInt32) ||
                type == typeof(UInt64) ||
                type == typeof(char) ||
                type == typeof(byte) ||
                type == typeof(bool);
        }
        public static bool IsIntegralValue(IConvertible value)
        {
            bool result = false;
            if (IsIntegralType(value))
            {
                result=true;
            } else {
                try
                {
                    double dblVal = (double)Convert.ChangeType(value, typeof(double));
                    double intVal = Math.Floor(dblVal);
                    return intVal == dblVal;
                }
                catch
                {
                    result = false;
                }
            }
            return result;

        }
        public static bool IsNumericType<T>()
        {
            return IsNumericType(typeof(T));
        }
        public static bool IsNumericType(object obj)
        {
            Type t = Objects.GetUnderlyingType(obj.GetType());
            return IsNumericType(t);
        }
        /// <summary>
        /// Any true numeric primitive type, e.g. all except string, char & bool
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(Type type)
        {
            return type.IsPrimitive && !(type == typeof(string) || type == typeof(char) || type == typeof(bool));
        }
        /// <summary>
        /// Any primitive type that can be converted to a number, e.g. all except string. This just
        /// returns any primitive type that is not IEnumerable
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNumericConvertible(Type type)
        {
            return type.IsPrimitive && !(type  == typeof(string));
        }
        public static bool IsNullableType(Type type)
        {
            return Objects.IsNullableType(type);
        }
        public static bool IsText(object value)
        {
            Type t  = value.GetType();
            return t == typeof(string) || t == typeof(char);
        }
        public static IFunction GetFunction<T>(string functionName)
        {
            bool IsTyped = typeof(T) == typeof(IConvertible);
            switch (functionName)
            {
                case "abs":
                    return new Functions.Abs();
                default:
                    throw new ArgumentException("Undefined function '" + functionName + "'");

            }
        }
        /// <summary>
        /// If the value is an operand, returns it, otherwise creates the right kind of operand
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IOperand EnsureOperand(IConvertible value)
        {
            if (value is IOperand)
            {
                return (IOperand)value;
            }
            else if (value is string)
            {
                //TODO: parse quotes and return a string liteeral if need be
                return Equations.CreateVariable((string)value);
            }
            else
            {
                return Equations.CreateLiteral(value);
            }
        }
    }
}
