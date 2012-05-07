using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser.Implementation.Functions
{

    public class Abs: Function
    {
        public Abs()
            : base("abs")
        {

        }

        protected override IConvertible GetValue()
        {
            IConvertible value = FirstOperand.Value;

            if (Utils.IsIntegralType(value))
            {
                return Math.Abs(Convert.ToInt64(value));
            }
            else
            {
                return Math.Abs(Convert.ToDecimal(value));
            }
        }
       
        
        public override int RequiredParmCount
        {
            get { return 1; }
        }

        public override int MaxParmCount
        {
            get { return 1; }
        }

        public override AssociationType AssociationType
        {
            get { return AssociationType.Function; }
        }

        protected override IOperand GetNewInstance()
        {
            return new Abs();
        }

        //protected override IOperand<U> CloneAsImpl<U>()
        //{
        //    throw new NotImplementedException();
        //}
    }

    //public class Abs<T> : Abs, IFunction<T> where T: IConvertible
    //{

    //    public new T Value
    //    {
    //        get { 
    //            return (T)GetValue(); 
    //        }
    //    }

    //    public new IOperand<T> Clone()
    //    {
    //        return new Abs<T>();
    //    }
    //}
}
