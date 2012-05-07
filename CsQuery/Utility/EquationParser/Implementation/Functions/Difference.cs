using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser.Implementation.Functions
{

    public class Difference : Sum
    {
        public Difference(IOperand operand1, IOperand operand2)
            : base()
        {
            AddOperand(operand1);
            AddOperand(operand2);
        }

        protected override OperationType PrimaryOperator
        {
            get
            {
                return OperationType.Subtraction;
            }
        }
        protected override OperationType ComplementaryOperator
        {
            get
            {
                return OperationType.Addition;
            }
        }
    }
    public class Difference<T> : Difference, IFunction<T> where T : IConvertible
    {
        public Difference(IOperand operand1, IOperand operand2)
            : base(operand1, operand2)
        { }

        public new T Value
        {
            get { return (T)GetValue(); }
        }


        public new Difference<T> Clone()
        {
            return (Difference<T>)Clone();
        }

        IOperand<T> IOperand<T>.Clone()
        {
            return Clone();
        }
    }
}
