using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser.Implementation.Functions
{

    public class Sum : NativeOperation, IOperation
    {
        public Sum()
            : base("sum")
        {

        }
        public Sum(params IConvertible[] operands)
            : base("sum", operands)
        {
        }

        public override AssociationType AssociationType
        {
            get { return AssociationType.Addition; }
        }
        protected override IOperand GetNewInstance()
        {
            return new Sum();
        }
        protected override OperationType PrimaryOperator
        {
            get { return OperationType.Addition; }
        }
        protected override OperationType ComplementaryOperator
        {
            get { return OperationType.Subtraction; }
        }
    }
    public class Sum<T> : Sum, IFunction<T> where T : IConvertible
    {
        public Sum()
            : base()
        {
        }

        public Sum(params IConvertible[] operands)
            : base( operands)
        {
        }
 
        public new T Value
        {
            get { return (T)Convert.ChangeType(GetValue(),typeof(T)); }
        }

        protected override IOperand GetNewInstance()
        {
            return new Sum<T>();
        }

        public new Sum<T> Clone()
        {
            return (Sum<T>)Clone();
        }

        IOperand<T> IOperand<T>.Clone()
        {
            return Clone();
        }
    }

}
