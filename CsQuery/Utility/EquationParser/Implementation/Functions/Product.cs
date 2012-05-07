using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser.Implementation.Functions
{

    public class Product : NativeOperation, IOperation
    {
        public Product()
            : base("product")
        {

        }
        public Product(params IConvertible[] operands)
            : base("product", operands)
        {
        }

        public override AssociationType AssociationType
        {
            get { return AssociationType.Multiplicaton ; }
        }
        protected override IOperand GetNewInstance()
        {
            return new Product();
        }
        protected override OperationType PrimaryOperator
        {
            get { return OperationType.Multiplication; }
        }
        protected override OperationType ComplementaryOperator
        {
            get { return OperationType.Division; }
        }
    }
    public class Product<T> : Product, IFunction<T> where T : IConvertible
    {
        public Product()
            : base()
        { }

        public Product(params IConvertible[] operands)
            : base(operands)
        { }

        public new T Value
        {
            get { return (T)GetValue(); }
        }

        protected override IOperand GetNewInstance()
        {
            return new Product();
        }
        public new Product<T> Clone()
        {
            return (Product<T>)Clone();
        }

        IOperand<T> IOperand<T>.Clone()
        {
            return Clone();
        }
    }

}
