using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.EquationParser.Implementation
{
    public abstract class Clause
    {
        public Clause()
        {

        }
        public Clause(IConvertible operandA)
        {
            OperandA = Equations.CreateOperand(operandA); 
        }
        public Clause(IConvertible operandA, IConvertible operandB, IOperator op)
        {
            CreateClause(operandA, operandB, op);
        }

        public Clause(IConvertible operandA, IConvertible operandB, string op)
        {
            CreateClause(operandA, operandB, new Operator(op));

        }
        protected void CreateClause(IConvertible operandA, IConvertible operandB, IOperator op) 
        {
            OperandA = Equations.CreateOperand(operandA);
            OperandB = Equations.CreateOperand(operandB);
            Operator = op;
        }
        public IOperand OperandA
        {
            get;
            set;
        }

        public IOperand OperandB
        {
            get;
            set;
        }

        public IOperator Operator
        {
            get;
            set;
        }
        
    }

    public class Clause<T> : Clause,IClause<T> where T: IConvertible
    {
        public Clause(): base()
        {}
        public Clause(IConvertible operandA)
            : base(operandA) 
        {}
        public Clause(IConvertible operandA, IConvertible operandB, IOperator op) :
            base(operandA, operandB, op)
        { }

        public Clause(IConvertible operandA, IConvertible operandB, string op) :
            base(operandA, operandB, op)
        { }
        protected IFormatProvider format = System.Globalization.CultureInfo.CurrentCulture;
        /// <summary>
        /// Builds an expression by returning the open clause: either the same clause, with the first 
        /// operand replace by a new clause containing (oldOpA, operandB, op), or a new clause that
        /// has been added as OperandB to the previous clause. The operand returned is based on the
        /// associativity of the operator, and will result in a correctly constructed association chain.
        /// </summary>
        /// <param name="operandB">The new operand</param>
        /// <param name="op">The operator</param>
        /// <returns></returns>
        public IClause<T> Chain(IOperand operandB, IOperator op)
        {
            if (OperandA == default(IOperand))
            {
                throw new Exception("You can't chain unless the first operand is already set.");
            }
            
            if (op.IsAssociative)
            {
                // a * b + c -- becomes (a*b) ... _
               
                IClause<T> newClause = new Clause<T>(OperandA,operandB,op);
                OperandA = newClause;
                return this;
            }
            else
            {
                // a + b * c -- becomes a + b ... _                 
                Operator = op;
                IClause<T> newClause = new Clause<T>();
                newClause.OperandA = operandB;
                OperandB = newClause;
                return newClause;
            }
        }
        
        IClause IClause.Chain(IOperand operandB, IOperator op)
        {
            return Chain(operandB, op);
        }
        protected Literal<T> output
        {
            get
            {
                if (_output == null)
                {
                    _output = new Literal<T>();
                }
                return _output;
            }
        }
        private Literal<T> _output;
        protected void Calculate()
        {
            if (OperandB == null && Operator==null)
            {
                output.Set((T)(IConvertible)(OperandA.Value.ToInt32(format)));

            }
            else if (Operator == null)
            {
                throw new Exception("This clause has two operands, but no operation.");
            }
            else
            {
                switch (Operator.Operation)
                {
                    case Operation.Addition:
                        if (output.IsInteger)
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToInt32(format) + OperandB.Value.ToInt32(format)));
                        }
                        else
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToDouble(format) + OperandB.Value.ToDouble(format)));
                        }
                        break;
                    case Operation.Subtraction:
                        if (output.IsInteger)
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToInt32(format) - OperandB.Value.ToInt32(format)));
                        }
                        else
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToDouble(format) - OperandB.Value.ToDouble(format)));
                        }
                        break;
                    case Operation.Multiplication:
                        if (output.IsInteger)
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToInt32(format) * OperandB.Value.ToInt32(format)));
                        }
                        else
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToDouble(format) * OperandB.Value.ToDouble(format)));
                        }
                        break;
                    case Operation.Division:
                        if (output.IsInteger)
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToInt32(format) / OperandB.Value.ToInt32(format)));
                        }
                        else
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToDouble(format) / OperandB.Value.ToDouble(format)));
                        }
                        break;
                    case Operation.Modulus:
                        if (output.IsInteger)
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToInt32(format) % OperandB.Value.ToInt32(format)));
                        }
                        else
                        {
                            output.Set((T)(IConvertible)(OperandA.Value.ToDouble(format) % OperandB.Value.ToDouble(format)));
                        }
                        break;
                    case Operation.Power:
                        output.Set((T)(IConvertible)(Math.Pow(OperandA.Value.ToDouble(format), OperandB.Value.ToDouble(format))));
                        break;
                    default:
                        throw new Exception("Unknown operation type applied to operands.");

                }
            }
        }

        public T Value
        {
            get
            {
                Calculate();
                return output.Value;
            }
        }

        IConvertible IOperand.Value
        {
            get
            {
                Calculate();
                return output.Value;
            }
        }

        bool IOperand.IsInteger
        {
            get { return output.IsInteger; }
        }

        IOperand IClause.OperandA
        {
            get
            {
                return OperandA;

            }
            set
            {
                OperandA = value;
            }
        }

        IOperand IClause.OperandB
        {
            get
            {
                return OperandB;
            }
            set
            {
                OperandB = value;
            }
        }

        public override string ToString()
        {
            string output = WrapOperand(OperandA);
            if (OperandB != null && Operator != null)
            {
                output += Operator.ToString() + WrapOperand(OperandB);
            }
            return output;
        }
        protected string WrapOperand(IOperand operand)
        {
            if (operand is IClause && !((IClause)operand).Operator.IsAssociative)
            {
                return "(" + operand.ToString() + ")";
            }
            else
            {
                return operand.ToString();
            }

        }

        #region IConvertible members
        TypeCode IConvertible.GetTypeCode()
        {
            return output.GetTypeCode();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return output.ToDouble(format);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return output.ToInt16(format);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return output.ToInt32(format);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return output.ToInt64(format);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
